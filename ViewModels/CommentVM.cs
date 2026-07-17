using System.ComponentModel.DataAnnotations;

namespace company_profile
{
   public class CommentVM
   {
      public int IDComment { get; set; }
      public int IDNews { get; set; }
      public int UserID { get; set; }
      public string? UserName { get; set; }
      
      [Required(ErrorMessage = "Komentar tidak boleh kosong")]
      public string? IsiKomentar { get; set; }
      public DateTime CreateAt { get; set; }
   }
}
