"use strict";
// Class definition
const arrangeTableBody = document.getElementById('arrange-table-tbody')
var KTUsersQuestion = function () {
    // Shared variables

    const element = document.getElementById('kt_modal_update_question');
    const form = document.getElementById('kt_modal_update_question_form');
    const modal = new bootstrap.Modal(element);
    // Init add schedule modal
    var initUpdateQuestion = () => {
        const submitButton = element.querySelector('[data-kt-question-modal-action="submit"]');
        const cancelButton = element.querySelector('[data-kt-question-modal-action="cancel"]');
        const closeButton = element.querySelector('[data-kt-question-modal-action="close"]');


        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        try {
            var validator = FormValidation.formValidation(
                form,
                {
                    fields: {
                        'QuestionType': {
                            validators: {
                                notEmpty: {
                                    message: 'Sorutipini giriniz alanı zorunludur'
                                }
                            }
                        },
                        'Content': {
                            validators: {
                                notEmpty: {
                                    message: 'İçerik alanı zorunludur'
                                }
                            }
                        },
                        'NewImage': {
                            validators: {
                                file: {
                                    maxSize: 1024 * 1024, // 1 MB
                                    message: 'Dosya boyutu 1 MB\'ı geçemez'
                                },
                                file: {
                                    extension: 'jpg,jpeg,png', // İzin verilen dosya türleri
                                    message: 'Sadece JPG, JPEG ve PNG dosyaları desteklenmektedir'
                                }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger(),
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: '.fv-row',
                            eleInvalidClass: '',
                            eleValidClass: ''
                        })
                    }
                }
            );
        } catch (e) {
            console.log("hata")
        }

        // Submit button handler
        submitButton.addEventListener('click', e => {
            e.preventDefault();

            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
                        // Show loading indication
                        submitButton.setAttribute('data-kt-indicator', 'on');

                        // Disable button to avoid multiple click 
                        submitButton.disabled = true;

                        // Simulate form submission. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                        setTimeout(function () {
                            // Remove loading indication
                            submitButton.removeAttribute('data-kt-indicator');

                            // Enable button
                            submitButton.disabled = false;

                            // Show popup confirmation 
                            Swal.fire({
                                text: localizedTexts.formSubmittedText,
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: localizedTexts.okButtonText,
                                customClass: {
                                    confirmButton: "btn btn-primary"
                                }
                            }).then(function (result) {
                                if (result.isConfirmed) {
                                    modal.hide();
                                }
                            });
                            document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
                            // Submit form
                            form.submit();
                        }, 2000);
                    } else {
                        // Show popup warning. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                        Swal.fire({
                            text: "Üzgünüm, bazı hatalar tespit edildi. Lütfen tekrar deneyin.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Anladım, tamam!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });
        // Cancel button handler
        cancelButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                title: localizedTexts.unsavedChangesTitle,
                text: localizedTexts.unsavedChangesText,
                icon: 'warning',
                showCancelButton: true,
                cancelButtonColor: '#3085d6',
                confirmButtonColor: '#d33',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();

                    $(form).find('.invalid-feedback').text("");

                    // Tüm form alanlarının kenarlıklarını temizle

                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Formunuz iptal edilmedi!",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Anladım, tamam!",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        });

        // Close button handler

        closeButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                title: localizedTexts.unsavedChangesTitle,
                text: localizedTexts.unsavedChangesText,
                icon: 'warning',
                showCancelButton: true,
                cancelButtonColor: '#3085d6',
                confirmButtonColor: '#d33',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();
                    $(form).find('.invalid-feedback').text("");
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Formunuz iptal edilmedi!",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Anladım, tamam!",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        });


    }
    return {
        // Public functions
        init: function () {
            initUpdateQuestion();
        }
    };
}();


// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsersQuestion.init();
});
