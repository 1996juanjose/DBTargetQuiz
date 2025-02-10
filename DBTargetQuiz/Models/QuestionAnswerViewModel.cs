namespace DBTargetQuiz.Models
{
    public class QuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string? QuestionDesc { get; set; }
        public string? QuestionPicture { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }
}
