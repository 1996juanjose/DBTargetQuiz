using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAcces
{
    public class QuestionDA
    {
        private readonly string _connectionString;

        public QuestionDA(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Question> GetQuestionsWithAnswers()
        {
            var questions = new Dictionary<int, Question>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetQuestionsWithAnswers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int questionId = Convert.ToInt32(reader["question_id"]);
                            if (!questions.ContainsKey(questionId))
                            {
                                questions[questionId] = new Question
                                {
                                    QuestionId = questionId,
                                    QuestionDesc = reader["question_desc"].ToString(),
                                    QuestionPicture = reader["question_picture"].ToString(),
                                    Answers = new List<Answer>()
                                };
                            }

                            questions[questionId].Answers.Add(new Answer
                            {
                                AnswerId = Convert.ToInt32(reader["answer_id"]),
                                AnswerDesc = reader["answer_desc"].ToString()
                            });
                        }
                    }
                }
            }

            return new List<Question>(questions.Values);
        }


        public int ProcesarCuestionario(Dictionary<int, int> respuestas, out string quizCode)
        {
            int quizId = 0;
            quizCode = string.Empty;

         
            string xmlRespuestas = ConvertirDiccionarioAXml(respuestas);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("upProcessQuiz", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                
                    cmd.Parameters.Add(new SqlParameter("@panswers", SqlDbType.Xml)
                    {
                        Value = xmlRespuestas
                    });

                    SqlParameter pquiz_id = new SqlParameter("@pquiz_id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    SqlParameter pquiz_code = new SqlParameter("@pquiz_code", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(pquiz_id);
                    cmd.Parameters.Add(pquiz_code);

                    cmd.ExecuteNonQuery();

                    quizId = (int)pquiz_id.Value;
                    quizCode = pquiz_code.Value.ToString();
                }
            }

            return quizId;
        }
        private string ConvertirDiccionarioAXml(Dictionary<int, int> respuestas)
        {
            XDocument xmlDoc = new XDocument(
                new XElement("Answers",
                    respuestas.Select(kv =>
                        new XElement("Answer",
                            new XAttribute("question_id", kv.Key),
                            new XAttribute("answer_id", kv.Value)
                        )
                    )
                )
            );

            return xmlDoc.ToString();
        }
        public List<Question> ObtenerPreguntas()
        {
            List<Question> questions = new List<Question>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("upGetQuestions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, Question> questionMap = new Dictionary<int, Question>();

                        while (reader.Read())
                        {
                            int questionId = Convert.ToInt32(reader["QuestionId"]);
                            if (!questionMap.ContainsKey(questionId))
                            {
                                questionMap[questionId] = new Question
                                {
                                    QuestionId = questionId,
                                    QuestionDesc = reader["QuestionDesc"].ToString(),
                                    Answers = new List<Answer>()
                                };
                            }

                            questionMap[questionId].Answers.Add(new Answer
                            {
                                AnswerId = Convert.ToInt32(reader["AnswerId"]),
                                AnswerDesc = reader["AnswerDesc"].ToString()
                            });
                        }

                        questions = new List<Question>(questionMap.Values);
                    }
                }
            }

            return questions;
        }
        public Candidate ObtenerCandidatoPorQuiz(int quizId)
        {
            Candidate candidate = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("upGetCandidateByQuiz", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@quiz_id", quizId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            candidate = new Candidate
                            {
                                CandidateName = reader["candidate_name"].ToString(),
                                CandidateGovPlan = reader["candidate_gov_plan"].ToString()
                            };
                        }
                    }
                }
            }

            return candidate;
        }

    }
}
