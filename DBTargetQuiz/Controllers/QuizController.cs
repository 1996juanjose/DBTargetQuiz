using BussinesLogic;
using DataAcces;
using DBTargetQuiz.Models;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace DBTargetQuiz
{
    public class QuizController : Controller
    {
        private readonly QuestionBL _questionBL;

        public QuizController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _questionBL = new QuestionBL(connectionString);
        }

        public IActionResult Index()
        {
            List<Question> questions = _questionBL.GetQuestionsWithAnswers();
            return View(questions);
        }


        [HttpPost]
        public IActionResult SubmitQuiz(Dictionary<int, int> answers)
        {
            if (answers == null || answers.Count == 0)
            {
                ModelState.AddModelError("", "Debe responder todas las preguntas.");
                return View("Quiz", _questionBL.ObtenerPreguntas());
            }

            string quizCode;
            int quizId = _questionBL.GuardarCuestionario(answers, out quizCode);
            var Candidato = _questionBL.ObtenerCandidatoPorQuiz(quizId);


            return RedirectToAction("Resultado", new
            {
                nombre = Candidato.CandidateName,
                ruta = Candidato.CandidateGovPlan
            });
        }
             
        public ActionResult Resultado(string nombre,string ruta)
        {
            ViewBag.Nombre = nombre;
            ViewBag.RutaImagen = ruta;
            return View();
        }
    }
}
