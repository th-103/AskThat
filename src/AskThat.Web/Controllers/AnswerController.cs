using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AskThat.Application.Interfaces;
using AskThat.Web.Models.ViewModels;

namespace AskThat.Web.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly ILogger<AnswersController> _logger;

        public AnswersController(IAnswerService answerService, ILogger<AnswersController> logger)
        {
            _answerService = answerService ?? throw new ArgumentNullException(nameof(answerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST: /Answers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnswerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide a valid answer.";
                return RedirectToAction("Details", "Questions", new { id = model.QuestionId });
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _answerService.CreateAnswerAsync(userId, model.QuestionId, model.Content);

                TempData["SuccessMessage"] = "Your answer has been posted successfully!";
                return RedirectToAction("Details", "Questions", new { id = model.QuestionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating answer for question {QuestionId}", model.QuestionId);
                TempData["ErrorMessage"] = "An error occurred while posting your answer.";
                return RedirectToAction("Details", "Questions", new { id = model.QuestionId });
            }
        }

        // GET: /Answers/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var answer = await _answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                {
                    return NotFound();
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (answer.UserId != userId)
                {
                    return Forbid();
                }

                var viewModel = new EditAnswerViewModel
                {
                    AnswerId = answer.AnswerId,
                    Content = answer.Content,
                    QuestionId = answer.QuestionId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving answer for edit {AnswerId}", id);
                return NotFound();
            }
        }

        // POST: /Answers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAnswerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var answer = await _answerService.UpdateAnswerAsync(model.AnswerId, userId, model.Content);

                if (answer == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Your answer has been updated successfully!";
                return RedirectToAction("Details", "Questions", new { id = model.QuestionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating answer {AnswerId}", model.AnswerId);
                ModelState.AddModelError("", "An error occurred while updating your answer.");
                return View(model);
            }
        }

        // POST: /Answers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int questionId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _answerService.DeleteAnswerAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Answer deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to delete the answer.";
                }

                return RedirectToAction("Details", "Questions", new { id = questionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting answer {AnswerId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the answer.";
                return RedirectToAction("Details", "Questions", new { id = questionId });
            }
        }
    }
}