using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es un dato obligatorio")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido un dato obligatorio")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Nombre Completo")]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Required(ErrorMessage = "El nombre de usuario es un dato obligatorio")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]        
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [MinLength(10)]        
        public string PasswordConfirmation { set; get; }

        [Required(ErrorMessage = "El rol es un dato obligatorio")]
        [Display(Name = "Rol del usuario")]
        public Roles Rol { get; set; }

        [JsonIgnore]
        public virtual UserData UserData { get; set; }

        [JsonIgnore]
        public virtual Session Session { get; set; }
                

        [Display(Name = "Activo")]
        public bool Enabled { get; set; }

        [JsonIgnore]
        public DateTime Created { get; set; }
    }

    [Table("UserData")]
    public class UserData
    {
        public int Id { get; set; }

        [InverseProperty("Id")]
        [ForeignKey("Id")]
        public virtual User User { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; }

        [DataType(DataType.EmailAddress,ErrorMessage ="Ingrese un correo electronico valido")]
        public String Email { get; set; }
    }

    [Table("Sessions")]
    public class Session
    {
        public int Id { get; set; }

        [InverseProperty("Id")]
        [ForeignKey("Id")]
        public virtual User User { get; set; }

        public string Token { get; set; }
    }
}
