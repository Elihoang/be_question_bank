using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEQuestionBank.Shared.DTOs.user
{
    public class UpdateCurrentUserDto
    {
        public string HoTen { get; set; }

        public string Email { get; set; }
    }
}
