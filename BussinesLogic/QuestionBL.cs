using DataAcces;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLogic
{
    public class QuestionBL
    {
        private readonly QuestionDA _questionDA;

        public QuestionBL(string connectionString)
        {
            _questionDA = new QuestionDA(connectionString);
        }

        public List<Question> GetQuestionsWithAnswers()
        {
            return _questionDA.GetQuestionsWithAnswers();
        }
        public int GuardarCuestionario(Dictionary<int, int> respuestas, out string quizCode)
        {
            if (respuestas == null || respuestas.Count == 0)
            {
                throw new ArgumentException("El cuestionario no puede estar vacío.");
            }

            return _questionDA.ProcesarCuestionario(respuestas, out quizCode);
        }

        public List<Question> ObtenerPreguntas()
        {
            return _questionDA.ObtenerPreguntas();
        }
        public Candidate ObtenerCandidatoPorQuiz(int quizId)
        {
            return _questionDA.ObtenerCandidatoPorQuiz(quizId);
        }
    }
}
