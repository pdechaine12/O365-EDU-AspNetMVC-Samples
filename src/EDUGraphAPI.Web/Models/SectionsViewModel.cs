using System.Collections.Generic;
using System.Linq;
using Microsoft.Education;
using Microsoft.Education.Data;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SectionsViewModel
    {
        public SectionsViewModel(string userEmail, School School, ArrayResult<Section> sections, IEnumerable<Section> mySections)
        {
            this.UserEmail = userEmail;
            this.School = School;
            this.Sections = sections;
            this.MySections = mySections.ToList();
        }

        public string UserEmail { get; set; }
        public School School { get; set; }
        public ArrayResult<Section> Sections { get; set; }
        public List<Section> MySections { get; set; }

        public bool IsMy(Section section)
        {
            return MySections != null && MySections.Any(c => c.Email == section.Email);
        }
    }
}