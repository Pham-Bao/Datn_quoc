var CartClass = {
    Get: function () {
        var sIDSP = localStorage.getItem("cart") || "[]";
        return JSON.parse(sIDSP);
    },
    Set: function (arr) {
        var jsonIDSP = JSON.stringify(arr);
        localStorage.setItem("cart", jsonIDSP);
    },

    UpdateQuantity: function (productId, newQuantity) {
        var cart = this.Get();
        var updatedCart = cart.map(function (item) {
            if (item.chiTietSPId === productId) {
                item.soLuong = newQuantity;
            }
            return item;
        });
        this.Set(updatedCart);
    },
    // Hàm addToCart để thêm sản phẩm vào giỏ hàng
    addToCart: function (productId, color, option, giaSP) {
        console.log('Thêm giỏ');
        console.log(productId, color, option, giaSP);

        var cart = this.Get();

        $.ajax({
            url: '/MuaHang/LayIdChiTietSP',
            type: 'POST',
            data: { id_sanPham: productId, mauSac: color, tuyChon: option },
            success: function (response) {
                if (response.success) {
                    var chiTietSPId = response.id_chiTietSP;
                    var existingItem = cart.find(item => item.chiTietSPId == chiTietSPId);
                    if (existingItem) {
                        $('#errorMessage').text('Sản phẩm đã có trong giỏ hàng.');
                        $('#errorAlert').fadeIn('slow').delay(2000).fadeOut('slow');
                        return;
                    }

                    var cartItem = {
                        chiTietSPId: chiTietSPId,
                        soLuong: 1,
                        giaSP: giaSP
                    };
                    cart.push(cartItem);

                    CartClass.Set(cart);
                    $('#successMessage').text('Sản phẩm đã được thêm vào giỏ hàng.');
                    $('#successAlert').fadeIn('slow').delay(2000).fadeOut('slow');
                } else {
                    $('#errorMessage').text('Không thể lấy được id_chiTietSP.');
                    $('#errorAlert').fadeIn('slow').delay(2000).fadeOut('slow');
                }
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi khi gửi yêu cầu AJAX để lấy id_chiTietSP
                $('#errorMessage').text('Đã xảy ra lỗi khi lấy id_chiTietSP: ' + error);
                $('#errorAlert').fadeIn('slow').delay(2000).fadeOut('slow');
            }
        });
    }



};
