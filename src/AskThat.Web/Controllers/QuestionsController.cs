using System.Security.Claims;
using AskThat.Application.Interfaces;
using AskThat.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskThat.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(IQuestionService questionService, ILogger<QuestionsController> logger)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: /Questions
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int limit = 20, string? search = null)
        {
            try
            {
                var questions = await _questionService.GetAllQuestionsAsync(page, limit, search);
                var questionViewModels = questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    Content = q.Content,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    Username = q.User?.Username ?? "Unknown",
                    UserId = q.UserId,
                    AnswerCount = q.AnswerCount
                });

                var viewModel = new QuestionsListViewModel
                {
                    Questions = questionViewModels,
                    CurrentPage = page,
                    PageSize = limit,
                    SearchTerm = search,
                    HasPreviousPage = page > 1,
                    HasNextPage = questionViewModels.Count() == limit
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions");
                return View("Error");
            }
        }

        // GET: /Questions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionWithAnswersAsync(id);
                if (question == null)
                {
                    return NotFound();
                }

                var viewModel = new QuestionViewModel
                {
                    QuestionId = question.QuestionId,
                    Title = question.Title,
                    Content = question.Content,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    AnswerCount = question.AnswerCount,
                    UserId = question.UserId,
                    Username = question.User?.Username ?? "Unknown",
                    Answers = question.Answers?.Select(a => new AnswerViewModel
                    {
                        AnswerId = a.AnswerId,
                        Content = a.Content,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt,
                        UserId = a.UserId,
                        Username = a.User?.Username ?? "Unknown",
                        QuestionId = a.QuestionId
                    }) ?? new List<AnswerViewModel>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question {QuestionId}", id);
                return NotFound();
            }
        }

        // GET: /Questions/Create
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Questions/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var question = await _questionService.CreateQuestionAsync(userId, model.Title, model.Content);

                TempData["SuccessMessage"] = "Question created successfully!";
                return RedirectToAction(nameof(Details), new { id = question.QuestionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                ModelState.AddModelError("", "An error occurred while creating the question.");
                return View(model);
            }
        }

        // GET: /Questions/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound();
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (question.UserId != userId)
                {
                    return Forbid();
                }

                var viewModel = new CreateQuestionViewModel
                {
                    Title = question.Title,
                    Content = question.Content
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question for edit {QuestionId}", id);
                return View("Error");
            }
        }

        // POST: /Questions/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var question = await _questionService.UpdateQuestionAsync(id, userId, model.Title, model.Content);

                if (question == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Question updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question {QuestionId}", id);
                ModelState.AddModelError("", "An error occurred while updating the question.");
                return View(model);
            }
        }

        // POST: /Questions/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _questionService.DeleteQuestionAsync(id, userId);

                if (!success)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Question deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question {QuestionId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the question.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}