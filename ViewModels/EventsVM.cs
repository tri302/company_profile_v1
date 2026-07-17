using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class EventsVM
    {
        public int IDEvents { get; set; }
        public string? UrlGambar { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? JudulEvent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? AlamatEvent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? IsiContentEvent { get; set; }
        
        //menampung filter multi-bahasa
        [NotMapped]
        public string? LokasiEvent { get; set; }
        public IFormFile? UploadGambar { get; set; }        

        [Display(Name = "Tanggal Mulai Event")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Tanggal Mulai Event wajib diisi")]
        public DateTime StartEvent { get; set; }

        [Display(Name = "Tanggal Selesai Event")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Tanggal Selesai Event wajib diisi")]
        public DateTime EndEvent { get; set; }
        
        // untuk menampilkan daftar event di halaman utama
        // Upcoming Events: Event yang akan datang (tanggal mulai > hari ini)
        // Past Events: Event yang sudah lewat (tanggal mulai <= hari ini)
        [NotMapped]
        public List<Events> UpcomingEvents { get; set; } = new List<Events>();
        [NotMapped]
        public List<Events> PastEvents { get; set; } = new List<Events>();
        [NotMapped]
        public IEnumerable<EventsVM> ListEvents { get; set; } = new List<EventsVM>();

        // Kolom Multi-Bahasa

        [Display(Name = "Judul Event (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Judul Event dalam Bahasa Indonesia wajib diisi")]
        public string? JudulEvent_ID { get; set; }

        [Display(Name = "Judul Event (Bahasa Inggris)")]
        [Required(ErrorMessage = "Judul Event dalam Bahasa Inggris wajib diisi")]
        public string? JudulEvent_EN { get; set; }

        [Display(Name = "Lokasi Event (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Lokasi Event dalam Bahasa Indonesia wajib diisi")]
        public string? LokasiEvent_ID { get; set; }

        [Display(Name = "Lokasi Event (Bahasa Inggris)")]
        [Required(ErrorMessage = "Lokasi Event dalam Bahasa Inggris wajib diisi")]
        public string? LokasiEvent_EN { get; set; }
        [Display(Name = "Alamat Event (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Alamat Event dalam Bahasa Indonesia wajib diisi")]
        public string? AlamatEvent_ID { get; set; }
        [Display(Name = "Alamat Event (Bahasa Inggris)")]
        [Required(ErrorMessage = "Alamat Event dalam Bahasa Inggris wajib diisi")]
        public string? AlamatEvent_EN { get; set; }
        [Display(Name = "Isi Content Event (Bahasa Indonesia)")]
        [Required(ErrorMessage = "Isi Content Event dalam Bahasa Indonesia wajib diisi")]
        public string? IsiContentEvent_ID { get; set; }
        [Display(Name = "Isi Content Event (Bahasa Inggris)")]
        [Required(ErrorMessage = "Isi Content Event dalam Bahasa Inggris wajib diisi")]
        public string? IsiContentEvent_EN { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }
        [ValidateNever]
        public int TotalPages { get; set; }
    }
}