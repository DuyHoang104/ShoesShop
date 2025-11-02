// JavaScript code for Cart page functionality (Address/shipping removed)
$(document).ready(function () {
    // ✅ Khởi tạo subtotal khi load trang
    updateSubtotal();

    // Cập nhật tổng tiền từng dòng
    function updateRowTotal(row) {
        const price = parseFloat(row.find(".product-price").text()) || 0;
        const qty = parseInt(row.find(".input-number").val()) || 0;
        const total = price * qty;
        row.find(".product-total").text(total.toFixed(2));
    }

    // Cập nhật subtotal (chỉ cộng các product-total, không tính shipping nữa)
    function updateSubtotal() {
        let subtotal = 0;
        $(".product-total").each(function () {
            const v = parseFloat($(this).text()) || 0;
            subtotal += v;
        });

        $("h5#subtotal").text(`$${subtotal.toFixed(2)}`);
    }

    // Tăng / Giảm số lượng
    $(document).on('click', '.input-number-increment', function () {
        const parent = $(this).closest('.product_count');
        const input = parent.find('.input-number');
        let value = parseInt(input.val()) || 0;
        const max = parseInt(input.attr('max')) || 10;
        if (value < max) {
            input.val(value + 1);
            const row = parent.closest("tr");
            updateRowTotal(row);
            updateSubtotal();
        }
    });

    $(document).on('click', '.input-number-decrement', function () {
        const parent = $(this).closest('.product_count');
        const input = parent.find('.input-number');
        let value = parseInt(input.val()) || 0;
        const min = parseInt(input.attr('min')) || 1;
        if (value > min) {
            input.val(value - 1);
            const row = parent.closest("tr");
            updateRowTotal(row);
            updateSubtotal();
        }
    });

    // Nếu người dùng chỉnh trực tiếp input số lượng thì cập nhật
    $(document).on('change', '.input-number', function () {
        const row = $(this).closest("tr");
        // đảm bảo giá trị hợp lệ
        let val = parseInt($(this).val()) || 1;
        const min = parseInt($(this).attr('min')) || 1;
        const max = parseInt($(this).attr('max')) || 10;
        if (val < min) val = min;
        if (val > max) val = max;
        $(this).val(val);

        updateRowTotal(row);
        updateSubtotal();
    });

    // (Optional) Khi trang load, đảm bảo các row total khớp với quantity hiện tại
    $(".product_count").each(function () {
        const row = $(this).closest("tr");
        updateRowTotal(row);
    });
    updateSubtotal();
});
/* End cart JS (address/shipping removed) */


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
