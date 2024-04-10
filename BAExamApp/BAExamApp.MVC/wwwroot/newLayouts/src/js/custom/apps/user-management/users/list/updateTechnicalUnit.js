﻿$('#updateTechnicalUnitModal').on('show.bs.modal', function (event) {
    let button = $(event.relatedTarget);
    let technicalUnitId = button.data('city-id');
    let technicalUnitName = button.data('city-name');

    let modal = $(this);
    modal.find('input[name="Name"]').val(technicalUnitName);
    modal.find('input[name="Id"]').val(technicalUnitId);
});

// Alert
function showModalCloseConfirmation() {
    Swal.fire({
        title: 'Değişiklikleri kaydetmediniz!',
        text: "Bu sayfadan çıkmak istediğinize emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, çık!',
        cancelButtonText: 'Hayır, kalmak istiyorum'
    }).then((result) => {
        if (result.isConfirmed) {
            // Event handler'ı geçici olarak kaldır
            $('#updateTechnicalUnitModal').off('hide.bs.modal');
            // Modalı kapat
            $('#updateTechnicalUnitModal').modal('hide');
            // Modal tamamen kapandıktan sonra event handler'ı tekrar ekle
            $('#updateTechnicalUnitModal').on('hidden.bs.modal', function () {
                addModalCloseConfirmation();
                $(this).find('form').trigger('reset');
            });
        }
    });
}

function addModalCloseConfirmation() {
    $('#updateTechnicalUnitModal').on('hide.bs.modal', function (e) {
        e.preventDefault();
        showModalCloseConfirmation();
    });
}

// İlk yüklemede event handler'ı ekle
addModalCloseConfirmation();

$('#updateTechnicalUnitModal').on('hidden.bs.modal', function () {
    $(this).find('form').trigger('reset');
});