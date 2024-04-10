$('#updateSubjectModal').on('show.bs.modal', function (event) {
    let button = $(event.relatedTarget);
    let subjectId = button.data('subject-id');
    let subjectName = button.data('subject-name');

    let modal = $(this);
    modal.find('input[name="Name"]').val(subjectName);
    modal.find('input[name="Id"]').val(subjectId);
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
            $('#updateSubjectModal').off('hide.bs.modal');
            // Modalı kapat
            $('#updateSubjectModal').modal('hide');
            // Modal tamamen kapandıktan sonra event handler'ı tekrar ekle
            $('#updateSubjectModal').on('hidden.bs.modal', function () {
                addModalCloseConfirmation();
                $(this).find('form').trigger('reset');
            });
        }
    });
}

function addModalCloseConfirmation() {
    $('#updateSubjectModal').on('hide.bs.modal', function (e) {
        e.preventDefault();
        showModalCloseConfirmation();
    });
}

// İlk yüklemede event handler'ı ekle
addModalCloseConfirmation();

$('#updateSubjectModal').on('hidden.bs.modal', function () {
    $(this).find('form').trigger('reset');
});