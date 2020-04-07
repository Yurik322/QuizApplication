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

        public AdminController(IGetInfoService getInfoService, IAdvancedMapper advancedMapper,
            IAdvancedLogicService advancedLogicService, IMapper mapper)
        {
            _getInfoService = getInfoService;
            _advancedMapper = advancedMapper;
            _advancedLogicService = advancedLogicService;
            _mapper = mapper;
        }

        /// <summary>
        /// Button "|Quiz app|".
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Якось викликати        http://localhost:53029/Admin/GetAllTestingResults
            // GetAllTestingResults();

            return View("ResultManagement");
        }

        // ToDo:
        /// <summary>
        /// Button "|Tests|".
        /// </summary>
        /// <returns></returns>
        public ActionResult TestManagement()
        {
            return View();
        }

        /// <summary>
        /// Button "|Testing Urls|".
        /// </summary>
        /// <returns></returns>
        public ActionResult TestingUrlManagement()
        {
            return View();
        }

        /// <summary>
        /// Button "|Results|".
        /// </summary>
        /// <returns></returns>
        public ActionResult ResultManagement()
        {
            var allResults =
                _getInfoService.GetAllTestingResults()
                    .Select(r => _mapper.Map<TestingResultViewModel>(r))
                    .ToList();

            return View(allResults);
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
    }
}