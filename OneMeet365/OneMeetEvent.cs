using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneMeet365
{
    public class OneMeetEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Time { get; set; }
        public string MeetingPlace { get; set; }
        public string EventPlace { get; set; }
        public string Activity { get; set; }
        public int MaxNumberOfPeople { get; set; }
    }
}
