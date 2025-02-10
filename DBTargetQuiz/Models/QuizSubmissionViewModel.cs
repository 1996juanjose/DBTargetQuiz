namespace DBTargetQuiz.Models
{
    public class QuizSubmissionViewModel
    {
        public Dictionary<int, int> Answers { get; set; } = new Dictionary<int, int>();
    }
}
