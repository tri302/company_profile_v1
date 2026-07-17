# Company Profile Web Application

[![Framework: .NET 9.0](https://img.shields.io/badge/Framework-.NET%209.0-purple.svg)](https://dotnet.microsoft.com/)
[![Database: MySQL / SQL Server](https://img.shields.io/badge/Database-MySQL%20%7C%20SQL%20Server-blue.svg)]()

## 📌 Tentang Proyek

Di era transformasi digital, impresi pertama sebuah instansi atau korporasi ditentukan oleh kehadiran platform digitalnya. **Company Profile Web Application** adalah solusi platform web modern berskala *enterprise* yang dirancang menggunakan ekosistem **.NET 9.0**. Aplikasi ini berfungsi sebagai instrumen bisnis strategis untuk menjembatani visi, misi, portofolio, dan layanan unggulan perusahaan Anda dengan calon klien, investor, dan audiens global secara interaktif.

Dikembangkan dengan arsitektur yang bersih (*clean architecture components*) dan standar performa tinggi, web aplikasi ini menyajikan antarmuka yang elegan di sisi pengguna (*frontend*) sekaligus menyediakan panel manajemen data (*backoffice*) yang aman, adaptif, dan responsif. Proyek ini siap membangun kredibilitas merek (*brand authority*) dan mengonversi kunjungan menjadi peluang bisnis yang nyata.

---

## 🚀 Keunggulan Utama Aplikasi

Web aplikasi ini dirancang khusus dengan memperhatikan aspek performa, keamanan, dan kemudahan skalabilitas:

1. **Performa Superior dengan .NET 9.0**: Memanfaatkan *runtime* tercepat di kelasnya untuk menjamin pemuatan halaman yang instan dan penanganan *request* yang efisien.
2. **Keamanan Berlapis (Enterprise-Grade Security)**: Dilengkapi proteksi otomatis terhadap kerentanan web utama, manajemen autentikasi bawaan, serta pembersihan input data yang ketat.
3. **Automated Background Jobs**: Eksekusi tugas-tugas latar belakang terkedwal (seperti pengiriman email masal atau pembersihan berkas) berjalan otomatis tanpa membebani performa utama web.
4. **Sistem Logging Terpusat (Advanced Diagnostic)**: Setiap riwayat aktivitas, eror aplikasi, dan metrik performa dicatat secara rapi demi kemudahan pemeliharaan (*maintenance*).
5. **Dual Database Compatibility**: Fleksibilitas tingkat tinggi yang mendukung integrasi dengan sistem database relasional populer baik di lingkungan *open-source* maupun korporat.

---

## 📦 Modul & Detail Fitur Unggulan

Sistem ini dipecah menjadi beberapa modul fungsional yang memastikan pengelolaan berjalan dinamis:

### 🌐 1. Modul Front-End & Interaksi Publik
* **Dynamic Hero & Profil Perusahaan**: Komponen visual utama di halaman beranda yang mengomunikasikan *Value Proposition* perusahaan seketika saat halaman dimuat.
* **Katalog Layanan & Produk Interaktif**: Menjabarkan solusi bisnis yang ditawarkan lengkap dengan manajemen konten yang fleksibel.
* **Galeri Portofolio & Rekam Jejak**: Ruang pameran keberhasilan proyek/klien terdahulu untuk meningkatkan aspek keandalan (*social proof*).
* **Formulir Kontak Terintegrasi (MailKit Inbound)**: Fitur bagi pengunjung untuk mengirimkan pesan atau keluhan yang langsung diproses oleh sistem backend.

### 🔐 2. Modul Autentikasi & Keamanan (Identity)
* **Manajemen Hak Akses & Pengguna**: Mengamankan area administrasi menggunakan enkripsi kata sandi *state-of-the-art* untuk mencegah akses ilegal.
* **Sanitasi Konten Otomatis**: Memastikan bahwa setiap input teks yang dimasukkan ke sistem (terutama teks kaya/HTML) bebas dari skrip berbahaya yang dapat memicu serangan siber.

### ⚙️ 3. Modul Otomatisasi & Pemeliharaan (Automation Hub)
* **Penjadwal Tugas (Quartz Background Engine)**: Modul internal yang mengelola tugas berkala seperti sinkronisasi data, pembaharuan berkas, dan pengiriman notifikasi tanpa intervensi manual.

---

## 🛠️ Detail Tech Stack & Fungsionalitas

Berikut adalah rincian komponen teknologi yang menyusun fondasi aplikasi ini:

### **Core Framework & Backend Logic**
* **.NET 9.0 (ASP.NET Core)**: *Framework* utama untuk memproses seluruh logika bisnis, pengelolaan rute (*routing*), dan penyajian data web dengan performa tinggi.
* **AngleSharp & AngleSharp.Css**: Digunakan untuk melakukan *parsing* dan manipulasi struktur HTML/CSS di sisi server, sangat berguna untuk pemrosesan konten dinamis.
* **Humanizer**: Library yang mengubah data teknis (seperti tipe data *DateTime*, angka, atau string) menjadi format bahasa yang ramah dan mudah dibaca oleh manusia (misal: "3 hari yang lalu").

### **Data Access & Databases**
* **Entity Framework Core (EF Core)**: Pustaka *Object-Relational Mapper* (ORM) utama yang menjembatani kode C# langsung ke tabel database tanpa perlu menulis kueri SQL manual.
* **EFCore.NamingConventions**: Memastikan standarisasi penamaan tabel dan kolom database secara otomatis (seperti konfigurasi *snake_case* atau *camelCase*).
* **Pomelo.EntityFrameworkCore.MySql & MySqlConnector**: *Provider* yang memungkinkan aplikasi terhubung dan mengoperasikan database **MySQL / MariaDB**.
* **Microsoft.Data.SqlClient**: *Provider* resmi untuk mendukung konektivitas dengan database **Microsoft SQL Server**.

### **Security & Utilities**
* **Microsoft.AspNetCore.Identity & UI**: Sistem manajemen user, *role-based access*, login, registrasi, dan validasi token keamanan yang sudah teruji.
* **HtmlSanitizer**: Alat pelindung yang bertugas membersihkan input teks HTML dari ancaman serangan *Cross-Site Scripting* (XSS).
* **MailKit & MimeKit**: Pustaka tangguh untuk menangani penyusunan pesan (*Mime Message*) dan pengiriman email otomatis dari aplikasi melalui protokol SMTP/IMAP.
* **Quartz.NET**: *Framework* penjadwalan kerja (*job scheduling*) berskala industri untuk mengeksekusi tugas di latar belakang (*background services*).

### **Diagnostics & Logging**
* **Serilog (dengan Sink Console, Debug, & File)**: Pustaka pencatatan log (*structured logging*) yang merekam segala aktivitas jalannya sistem ke terminal maupun ke dalam file fisik `.log` secara terstruktur untuk kebutuhan *debugging*.

---

## ⚙️ Panduan Instalasi & Cara Menjalankan

Ikuti langkah-langkah berikut untuk memasang dan menjalankan proyek di komputer lokal Anda:

### **Prasyarat Sistem (Prerequisites)**
Pastikan perangkat Anda sudah terpasang software berikut:
* **.NET 9.0 SDK** (Versi terbaru)
* Database Engine (**MySQL** melalui XAMPP/Laragon atau **SQL Server**)
* Kode Editor seperti **Visual Studio 2022** atau **VS Code** (beserta ekstensi C# Dev Kit)

### **Langkah-Langkah Menjalankan Aplikasi**

1. **Kloning Repositori**
   ```bash
   git clone [https://github.com/tri302/company_profile_v1.git](https://github.com/tri302/company_profile_v1.git)
   cd company_profile
