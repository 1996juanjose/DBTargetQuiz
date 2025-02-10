using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string? QuestionDesc { get; set; }
        public string? QuestionPicture { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}
