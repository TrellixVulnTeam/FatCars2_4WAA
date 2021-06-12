using System.Collections.Generic;

namespace FatCars.Application.Dtos
{
    public class UserDto
    {
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public string Login { get; set; }
        public string Registration { get; set; }

        public List<EmailDto> Emails { get; set; }

    }
}
