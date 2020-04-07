using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DAL.DataAccess;
using ModelClasses.Entities.Testing;
using ModelClasses.Entities.TestParts;
using QuizApp.ViewModel;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;

namespace QuizApp.Controllers
{
    [Authorize]
    public class ApilikeController : Controller
    {
        //private readonly IRepository<Test> _testRepository;
        private readonly IGetInfoService _getInfoService;
        private readonly ILowLevelTestManagementService _lowLevelTestManagementService;
        private readonly IHighLevelTestManagementService _highLevelTestManagementService;

        private readonly IMapper _mapper;
        private readonly IAdvancedMapper _advancedMapper;

        public ApilikeController(/*IRepository<Test> testRepository,*/ 
            IGetInfoService getInfoService,
            ILowLevelTestManagementService lowLevelTestManagementService,
            IHighLevelTestManagementService highLevelTestManagementService, IMapper mapper,
            IAdvancedMapper advancedMapper)
        {
            //_testRepository = testRepository;
            _getInfoService = getInfoService;
            _lowLevelTestManagementService = lowLevelTestManagementService;
            _highLevelTestManagementService = highLevelTestManagementService;
            _mapper = mapper;
            _advancedMapper = advancedMapper;
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
        public void CreateQuestion(string testGuid, QuestionViewModel question)     /*  ="daabda81-6ffa-4ec4-a578-d8e04c74d478" */
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


        [HttpPost]
        public void CreateTest(TestViewModel test)
        {
            var testFromDomain = _advancedMapper.MapTestViewModel(test);
            _highLevelTestManagementService.CreateTest(testFromDomain);
        }
        ////ToDo?????????????????????????????????????????????????????????????????????????????????????????????
        //// Мб заберу:
        //public Test GetTestByGuid(string testGuid)  // ne працює(це моя функція, її мб можна забрати)
        //{
        //    // ERROR!!!!!!!!!!!! (testGuid = null -- не можу обробити стрічку з javascript-ajax)
        //    return _testRepository.Get(test => test.Guid == testGuid);
        //}
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
        public void CreateTestingUrl(TestingUrlViewModel testingUrl)    //GUID????
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