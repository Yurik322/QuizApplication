using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DAL.DataAccess;
using ModelClasses.Entities.Testing;
using ModelClasses.Entities.TestParts;
using MoreLinq;
using QuizApp.ViewModel;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;

namespace QuizApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //ToDo: МБ можна уникнути оцього, але тоді треба поміняти оті кріейти
        TestingContext db = new TestingContext();   // Connect to DB.

        private readonly IGetInfoService _getInfoService;
        private readonly IAdvancedMapper _advancedMapper;
        private readonly IAdvancedLogicService _advancedLogicService;
        private readonly IMapper _mapper;


        
        private readonly ILowLevelTestManagementService _lowLevelTestManagementService;
        private readonly IHighLevelTestManagementService _highLevelTestManagementService;



        public AdminController(IGetInfoService getInfoService,
            ILowLevelTestManagementService lowLevelTestManagementService,
            IHighLevelTestManagementService highLevelTestManagementService, IMapper mapper,
            IAdvancedMapper advancedMapper)
        {
            _getInfoService = getInfoService;
            _lowLevelTestManagementService = lowLevelTestManagementService;
            _highLevelTestManagementService = highLevelTestManagementService;
            _mapper = mapper;
            _advancedMapper = advancedMapper;
        }


        public ActionResult Index()
        {
            // Якось викликати        http://localhost:53029/Admin/GetAllTestingResults
            // GetAllTestingResults();

            return View("ResultManagement");
        }

        //ToDo:
        public ActionResult TestManagement()
        {
            // Якось викликати        http://localhost:53029/Admin/GetAllTests
            // GetAllTests();

            var allTests = _getInfoService.GetAllTests().Select(t => _advancedMapper.MapTest(t)).ToList();
            return View(allTests);
        }

        [HttpGet]
        public ActionResult CreateTestingUrl()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTestingUrl(TestingUrl colect)
        {
            TryUpdateModel(colect);
            if (ModelState.IsValid)
            {
                db.TestingUrls.Add(colect);
                db.SaveChanges();

                return RedirectToAction("TestingUrlManagement");
            }

            return View();
        }

        public ActionResult TestingUrlManagement()
        {
            return View();
        }

        public ActionResult ResultManagement()
        {
            return View();
        }

//        http://localhost:53029/Admin/GetAllTests
        [HttpGet]
        public JsonResult GetAllTests()
        {
            var allTests = _getInfoService.GetAllTests().Select(t => _advancedMapper.MapTest(t)).ToList();
            return Json(allTests, JsonRequestBehavior.AllowGet);
        }

//        http://localhost:53029/Admin/GetAllTestingUrls
        [HttpGet]
        public JsonResult GetAllTestingUrls()
        {
            var testingsList = _getInfoService.GetAllTestingUrls();

            var parsedTestingsList = testingsList.Select(t => _advancedMapper.MapTestingUrl(t)).ToList();

            return Json(parsedTestingsList, JsonRequestBehavior.AllowGet);
        }

//        http://localhost:53029/Admin/GetAllTestingResults
        [HttpGet]
        public JsonResult GetAllTestingResults()
        {
            var allResults =
                _getInfoService.GetAllTestingResults()
                    .Select(r => _mapper.Map<TestingResultViewModel>(r))
                    .ToList();
            return Json(allResults, JsonRequestBehavior.AllowGet);
        }

        // Реалізовано - відразу скачує.
        public void GetResultsForTestCsv(string testGuid)
        {
            StringWriter oStringWriter = new StringWriter();
            oStringWriter.WriteLine("LoL line");
            Response.ContentType = "text/plain";

            Response.AddHeader("content-disposition", "attachment;filename=" +
                                                      $"test_results_for_{testGuid}.csv");
            Response.Clear();

            using (StreamWriter writer = new StreamWriter(Response.OutputStream, Encoding.UTF8))
            {
                _advancedLogicService.GetCsvResults(testGuid, writer);
            }
            Response.End();
        }




        
        [HttpGet]
        public JsonResult GetAnswersByQuestionGuid(string questionGuid)
        {
            var answerViewModelList = _getInfoService
                .GetQuestionByGuid(questionGuid)
                ?.TestAnswers
                .Select(a => _mapper.Map<AnswerViewModel>(a))
                .ToList();

            return Json(answerViewModelList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void CreateAnswer(string questionGuid, AnswerViewModel answer)
        {
            var testAnswer = _mapper.Map<TestAnswer>(answer);
            _lowLevelTestManagementService.CreateAnswerForQuestion(questionGuid, testAnswer);
        }
        [HttpPost]
        public void RemoveAnswer(string answerGuid)
        {
            _lowLevelTestManagementService.RemoveAnswer(answerGuid);
        }

        [HttpGet]
        public JsonResult GetQuestionsByTestGuid(string testGuid)
        {
            var questionViewModelList = _getInfoService
                .GetTestByGuid(testGuid)
                ?.TestQuestions
                .Select(q => _advancedMapper.MapTestQuestion(q))
                .ToList();

            return Json(questionViewModelList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void CreateQuestion(string testGuid, QuestionViewModel question)
        {
            var testQuestion = _mapper.Map<TestQuestion>(question);
            _lowLevelTestManagementService.CreateQuestionForTest(testGuid, testQuestion);
        }
        [HttpPost]
        public void RemoveQuestion(string testGuid, string questionGuid)
        {
            _lowLevelTestManagementService.RemoveQuestion(questionGuid);
        }
        [HttpPost]
        public void UpdateQuestion(string questionGuid, QuestionViewModel question)
        {
            var testQuestion = _mapper.Map<TestQuestion>(question);
            _lowLevelTestManagementService.UpdateQuestion(questionGuid, testQuestion);
        }


        //ToDo?????????????????????????????????????????????????????????????????????????????????????????????
        [HttpPost]
        public void CreateTest(TestViewModel test)
        {
            var testFromDomain = _advancedMapper.MapTestViewModel(test);
            _highLevelTestManagementService.CreateTest(testFromDomain);
        }
        [HttpPost]
        public void UpdateTest(string testGuid, TestViewModel test)
        {
            var testFromDomain = _advancedMapper.MapTestViewModel(test);
            _highLevelTestManagementService.UpdateTest(testGuid, testFromDomain);
        }
        [HttpPost]
        public void RemoveTest(string testGuid)
        {
            _highLevelTestManagementService.RemoveTest(testGuid);
        }


        [HttpPost]
        public void CreateTestingUrl(TestingUrlViewModel testingUrl)
        {
            var testUrlDomain = _advancedMapper.MapTestingUrlViewModel(testingUrl);
            _highLevelTestManagementService.CreateTestingUrl(testUrlDomain);
        }
        [HttpPost]
        public void RemoveTestingUrl(string testingUrlGuid)
        {
            _highLevelTestManagementService.RemoveTestingUrl(testingUrlGuid);
        }


        [HttpPost]
        public void RemoveTestingResult(string testingResultGuid)
        {
            _highLevelTestManagementService.RemoveTestingResult(testingResultGuid);
        }
    }
}