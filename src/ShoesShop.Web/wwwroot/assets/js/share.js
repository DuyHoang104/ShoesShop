/* JavaScript code for Order Checkout page functionality (kept as-is) */
// Cập nhật màu cho select country khi có giá trị
$('select.country_select').on('change', function () {
    var $this = $(this);
    var $nice = $this.next('.nice-select');
    if ($this.val()) {
        $nice.addClass('has-value');
    } else {
        $nice.removeClass('has-value');
    }
});

// Cập nhật trượt cho giá tiền của category filter
$(function () {
    if ($.fn.ionRangeSlider) {
        $(".js-range-slider").ionRangeSlider({
            type: "double",
            min: 0,
            max: 5000000,
            from: 500000,
            to: 2000000,
            step: 50000,
            prefix: "",
            onChange: function (data) {
                $(".js-input-from").val(data.from.toLocaleString());
                $(".js-input-to").val(data.to.toLocaleString());
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const titles = document.querySelectorAll(".l_w_title[data-bs-toggle='collapse']");
    titles.forEach(title => {
        const icon = title.querySelector(".toggle-icon");
        const targetId = title.getAttribute("data-bs-target");
        const target = document.querySelector(targetId);

        if (!target) return;

        target.addEventListener("hidden.bs.collapse", () => {
            if (icon) icon.textContent = "▲";
        });
        target.addEventListener("shown.bs.collapse", () => {
            if (icon) icon.textContent = "▼";
        });
    });
});
