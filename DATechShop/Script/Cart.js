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
    addToCart: function (productId, color, option) {
        console.log('Thêm giỏ');
        console.log(productId, color, option);

        var cart = this.Get();

    
        $.ajax({
            url: '/MuaHang/LayIdChiTietSP',
            type: 'POST',
            data: { id_sanPham: productId, mauSac: color, tuyChon: option },
            success: function (response) {
                // Xử lý phản hồi từ server
                if (response.success) {
                    var chiTietSPId = response.id_chiTietSP; 
                    var existingItem = cart.find(item => item.chiTietSPId == chiTietSPId);
                    console.log(existingItem)
                    if (existingItem) {
                        console.log('Sản phẩm đã có trong giỏ hàng okok');
                        return; // Không thêm sản phẩm nếu đã tồn tại trong giỏ hàng
                    }

                    // Lưu thông tin sản phẩm vào localStorage
                    var cartItem = {
                        chiTietSPId: chiTietSPId,
                        soLuong: 1, // Số lượng sản phẩm mặc định là 1 khi thêm vào giỏ hàng
                    };
                    cart.push(cartItem);
                    // Lưu giỏ hàng mới vào localStorage
                    CartClass.Set(cart);
                    console.log('Sản phẩm đã được thêm vào giỏ hàng ok ok ');
                } else {
                    // Xử lý khi không nhận được id_chiTietSP từ máy chủ
                    console.error('Không thể lấy được id_chiTietSP.');
                }
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi khi gửi yêu cầu AJAX để lấy id_chiTietSP
                console.error('Đã xảy ra lỗi khi lấy id_chiTietSP:', error);
            }
        });
    },


};
