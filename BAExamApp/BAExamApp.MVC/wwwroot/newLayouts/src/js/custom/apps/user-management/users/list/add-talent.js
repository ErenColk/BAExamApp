"use strict";

// Class definition
var KTUsersAddTalent = function () {
    // Shared variables
    const element = document.getElementById('kt_modal_add_talent');
    const form = document.getElementById('kt_modal_add_talent_form');
    const modal = new bootstrap.Modal(element);

    // Init add schedule modal
    var initAddUser = () => {

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/

        var validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'Name': {
                        validators: {
                            notEmpty: {
                                message: 'Yetenek Adı alanı zorunludur'
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


        // Submit button handler
        const submitButton = element.querySelector('[data-kt-users-modal-action="submit"]');
        submitButton.addEventListener('click', async e => {
            e.preventDefault();

            // Validate form before submit
            if (validator) {
                validator.validate().then(async function (status) {

                    if (status == 'Valid') {

                        await addNewTalent();
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
                                text: "Yetenek başarıyla eklendi",
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "Tamam, anlaşıldı",
                                customClass: {
                                    confirmButton: "btn btn-primary"
                                }
                            }).then(function (result) {
                                if (result.isConfirmed) {
                                    form.reset();
                                    modal.hide();
                                }
                            });
                            //form.submit(); // Submit form
                        }, 2000);
                    }
                });
            }
        });

        // Close button handler
        const closeButton = element.querySelector('[data-kt-talent-modal-action="close"]');
        closeButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                title: 'Değişiklikleri kaydetmediniz!',
                text: "Bu sayfadan çıkmak istediğinize emin misiniz?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Evet, çık!',
                cancelButtonText: 'Hayır, kalmak istiyorum'
            }).then((result) => {
                if (result.value) {
                    validator.resetForm();
                    form.reset(); // Reset form			
                    modal.hide();
                }
            });
        });
    }

    return {
        // Public functions
        init: function () {
            initAddUser();
        }
    };
}();

async function addNewTalent() {
    const names = document.getElementById('Name').value;
    const selectedTalents = [];
    let selectElement = "";
    let rootId = document.querySelector("a[data-bs-target='#kt_modal_add_talent']").getAttribute('data-root-id');

    if (rootId == 0) {
        $('select[name="TalentIds"] option:selected').each(function () {
            selectedTalents.push({ id: $(this).val(), name: $(this).text() });
        });
    } else if (rootId == 1) {
        $('select[name="TrainerTalentIds"] option:selected').each(function () {
            selectedTalents.push({ id: $(this).val(), name: $(this).text() });
        });
    }
    

    $.ajax({
        url: '/Admin/Trainer/AddTalent',
        method: "POST",
        data: { name: names },
        success: function (talents) {
            if (rootId == 0) {
                selectElement = document.querySelector('select[name="TalentIds"]');
            } else if (rootId == 1) {
                selectElement = document.querySelector('select[name="TrainerTalentIds"]');
            }

            selectElement.innerHTML = '';

            talents.forEach(talent => {
                const option = document.createElement('option');
                option.value = talent.id;
                option.text = talent.name;

                // Eğer bu yetenek daha önce seçiliyse, seçili yap
                const isAlreadySelected = selectedTalents.some(selected => selected.id === talent.id);
                if (isAlreadySelected) {
                    option.selected = true;
                }

                selectElement.appendChild(option);
            });

            if (typeof $ !== 'undefined' && $.fn.select2) {
                $(selectElement).trigger('change');
            }
        }
    });
}

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsersAddTalent.init();
});