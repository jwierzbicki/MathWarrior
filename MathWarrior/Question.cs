//*Copyright Jacek Wierzbicki*

namespace MathWarrior
{
    /// <summary>
    /// Question class/struct. Stores expression and answers.
    /// </summary>
    public class Question
    {
        /// <summary> Expression. </summary>
        public string QuestionString;
        /// <summary> Answers array. </summary>
        public string[] Answers;
        /// <summary> Correct option number (index of <see cref="MathWarrior.Question.Answers"/> array). </summary>
        public int CorrectOption; //Number of the correct answer

        /// <summary>
        /// Question constructor.
        /// </summary>
        /// <param name="QuestionString"></param>
        /// <param name="answers"></param>
        /// <param name="CorrectOption"></param>
        public Question(string QuestionString, string[] answers, int CorrectOption)
        {
            this.QuestionString = QuestionString;
            this.Answers = answers;
            this.CorrectOption = CorrectOption;
        }
    }
}
