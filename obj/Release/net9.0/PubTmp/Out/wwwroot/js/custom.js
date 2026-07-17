(function ($) {
    "use strict";

    $(document).ready(function () {
        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".portfolio-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            nav: true,
            navText: ["<span>&lsaquo;</span>", "<span>&rsaquo;</span>"],
            responsive: { 0: { items: 1 }, 600: { items: 1 }, 1000: { items: 1 } },
        });

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".project-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            nav: true,
            navText: ["<span>&lsaquo;</span>", "<span>&rsaquo;</span>"],
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 3 } },
        });

        // --- 1. Filterizr (Portfolio) ---
        if ($(".filtr-container").length > 0) {
            // Inisialisasi
            var filterizd = $(".filtr-container").filterizr({
                animationDuration: 0.5,
                setupControls: false,
            });

            // Handler Klik
            $("#filtrnav li").on("click", function () {
                $("#filtrnav li").removeClass("filtr-active");
                $(this).addClass("filtr-active");
                var filterValue = $(this).attr("data-filter");
                filterizd.filterizr("filter", filterValue);
            });
        }

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".slider-one").owlCarousel({
            items: 1,
            loop: true,
            autoplay: true,
            autoplayTimeout: 5000,
            nav: true,
            dots: true,
            animateOut: "fadeOut",
            navText: ["<span>&lsaquo;</span>", "<span>&rsaquo;</span>"],
        });

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".team-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            nav: true,
            dots: true,
            navText: ["<span>&lsaquo;</span>", "<span>&rsaquo;</span>"],
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 4 } },
        });

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".testimonial-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            dots: true,
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 2 } },
        });

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".blog-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            nav: true,
            navText: ["<span>&lsaquo;</span>", "<span>&rsaquo;</span>"],
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 3 } },
        });

        // Owl Carousel Inisialisasi
        // Membuat slidder dengan Owl Carousel
        $(".brand-carousel").owlCarousel({
            loop: true,
            autoplay: true,
            margin: 30,
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 5 } },
        });

        // --- 3. MeanMenu (Mobile) ---
        $(".main-menu").meanmenu({
            meanMenuContainer: ".mobile-menu",
            meanScreenWidth: "991",
            meanExpandableChildren: true,
            meanExpand: "+",
            meanContract: "-",
        });

        // Fungsi Scroll to Top
        $(window).scroll(function () {
            // Jika scroll lebih dari 300px, munculkan tombol
            if ($(this).scrollTop() > 300) {
                $(".scroll-top").fadeIn();
            } else {
                $(".scroll-top").fadeOut();
            }
        });

        $(".scroll-top").click(function () {
            // Animasi scroll ke atas dalam waktu 1 detik (1000ms)
            $("html, body").animate(
                {
                    scrollTop: 0,
                },
                1000,
            );
            return false;
        });
    });
    //End Document Ready

    // --- 4. Library Lainnya (Luar Ready) ---
    toastr.options = {
        progressBar: true,
        positionClass: "toast-top-right",
        timeOut: "3000",
    };
})(jQuery);

$(document).ready(function () {
    var headerHeight = $(".header-area").outerHeight();

    $(window).scroll(function () {
        if ($(window).scrollTop() > headerHeight) {
            $("#strickymenu").addClass("sticky");
        } else {
            $("#strickymenu").removeClass("sticky");
        }
    });
});

// Pengecekan Aman untuk Search Toggle
const searchToggle = document.getElementById("searchToggle");
if (searchToggle) {
    searchToggle.addEventListener("click", function () {
        const searchBox = document.getElementById("searchBox");
        if (searchBox) {
            if (searchBox.style.display === "none" || searchBox.style.display === "") {
                searchBox.style.display = "flex";
                const searchInput = searchBox.querySelector("input");
                if (searchInput) {
                    searchInput.focus();
                }
            } else {
                searchBox.style.display = "none";
            }
        }
    });
}

$(document).on("keydown", function (e) {
    if (e.key === "Escape") {
        $(".input-search").slideUp(200);
    }
});

// Pengecekan Aman untuk Newsletter Form
const newsletterForm = document.getElementById("newsletterForm");
if (newsletterForm) {
    newsletterForm.addEventListener("submit", function (e) {
        e.preventDefault();

        const emailInputEl = document.getElementById("newsletterEmail");
        const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');

        // Validasi apakah elemen input email dan token CSRF benar-benar ada di dalam form
        if (!emailInputEl || !tokenEl) {
            console.error("Elemen input email atau token CSRF tidak ditemukan.");
            return;
        }

        const emailInput = emailInputEl.value;
        const token = tokenEl.value;

        const formData = new URLSearchParams();
        formData.append("Email", emailInput);
        formData.append("__RequestVerificationToken", token);

        fetch("/Newsletter/Subscribe", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
            },
            body: formData.toString(),
        })
            .then((response) => {
                if (!response.ok) {
                    return response.json().then((err) => {
                        throw err;
                    });
                }
                return response.json();
            })
            .then((data) => {
                if (data.success) {
                    Swal.fire({
                        title: "Success!",
                        text: data.message,
                        icon: "success",
                        confirmButtonText: "OK",
                    });
                    newsletterForm.reset();
                }
            })
            .catch((error) => {
                const errorMsg =
                    error.message || "Terjadi kesalahan sistem. Silakan coba lagi.";
                Swal.fire({
                    title: "Error!",
                    text: errorMsg,
                    icon: "error",
                    confirmButtonText: "OK",
                });
                console.error("Error:", error);
            });
    });
}