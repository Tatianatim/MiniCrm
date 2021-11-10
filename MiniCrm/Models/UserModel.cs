using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniCrm.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [DisplayName("First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [DisplayName("Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(70)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [DisplayName("Phone")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address 1 is required.")]
        [DisplayName("Address 1")]
        [StringLength(50)]
        public string Address1 { get; set; }

        [DisplayName("Address 2")]
        [StringLength(50)]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [DisplayName("City")]
        [StringLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [DisplayName("State")]
        [StringLength(2)]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip is required.")]
        [DisplayName("Zip")]
        [StringLength(10)]
        public string Zip { get; set; }
    }
}