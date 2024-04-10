let subjectsReviewed = [];
let subtopicsReviewed = [];
let questionAnswerChoicesReviewed = [];
let questionDifficultiesReviewed = [];
let answerTypeNameReviewed = "Metin";
document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);

if ($("#productIdReviewed").val()) {
    onProductChangeReviewed().then(() => {
        document.getElementById("subjectIdReviewed").value = document.getElementById("subjectIdFromReloadReviewed").value;
        onSubjectChangeReviewed().then(() => {
            document.getElementById("subtopicIdReviewed").value = document.getElementById("subtopicIdFromReloadReviewed").value;
        });
    });
}

if ($("#questionTypeReviewed").val()) {
    questionAnswerChoicesReviewed = JSON.parse(document.getElementById("questionAnswersListFromReloadReviewed").value);
    createQuestionAnswersHtmlReviewed($("#questionTypeReviewed").val());
}

if ($("#questionDifficultyIdReviewed").val()) {
    onQuestionDifficultyChangeReviewed();
}

//Ajax functions for getting selectList
async function getSubjectsReviewed(selectedProductId) {
    return $.ajax({
        url: '/Trainer/Question/GetSubjectsByProductId',
        data: { productId: selectedProductId },
    });
}
async function getSubtopicsReviewed(selectedSubjectId) {
    return $.ajax({
        url: '/Trainer/Question/GetSubtopicsBySubjectId',
        data: { subjectId: selectedSubjectId },
    });
}
async function getTimeReviewed(selectedQuestionDifficultyId) {
    return $.ajax({
        url: '/Trainer/Question/GetQuestionTimeByDifficultyId',
        data: { difficultyId: selectedQuestionDifficultyId },
    });
}

async function getQuestionDifficultiesReviewed() {
    return $.ajax({
        url: '/Trainer/Question/GetQuestionDifficulties'
    });
}


//Product type change event.
async function onProductChangeReviewed() {
    subjectsReviewed = subjectsReviewed ? await getSubjectsReviewed($("#productIdReviewed").val()) : subjectsReviewed;
    populateSelectListReviewed("subjectIdReviewed", subjectsReviewed);
    subtopicsReviewed = [];
    populateSelectListReviewed("subtopicIdReviewed", subtopicsReviewed);
    removeChooseOptionReviewed("subjectIdReviewed");
    removeChooseOptionReviewed("subtopicIdReviewed");
};

//Product type change event.
async function onSubjectChangeReviewed() {
    subtopicsReviewed = subtopicsReviewed ? await getSubtopicsReviewed($("#subjectIdReviewed").val()) : subtopicsReviewed;
    populateSelectListReviewed("subtopicIdReviewed", subtopicsReviewed);
    removeChooseOptionReviewed("subtopicIdReviewed");
};

//Question type change event.
async function onQuestionTypeChangeReviewed() {

    document.getElementById("TimeGiven").value = "";
    questionDifficultiesReviewed = questionDifficultiesReviewed ? await getQuestionDifficultiesReviewed($("#questionTypeReviewed").val()) : questionDifficultiesReviewed;
    populateSelectListReviewed("questionDifficultyIdReviewed", questionDifficultiesReviewed);
    questionDifficultiesReviewed = [];

    let questionType = document.getElementById("questionTypeReviewed").value;

    if (questionType == 3) {
        questionAnswerChoicesReviewed = [{ Answer: "True", IsRightAnswer: false }, { Answer: "False", IsRightAnswer: false }];
    }
    else {
        questionAnswerChoicesReviewed = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }];
    }

    createQuestionAnswersHtmlReviewed(questionType);
};


//Question difficulty change event.
async function onQuestionDifficultyChangeReviewed() {
    let questionType = document.getElementById("questionTypeReviewed").value;

    if (questionType !== "4") {
        document.getElementById("TimeGiven").value = await getTimeReviewed($("#questionDifficultyIdReviewed").val());
    }
};

async function createQuestionAnswersHtmlReviewed(questionType) {
    switch (questionType) {
        case '1':
            {
                document.getElementById("questionAnswersDivReviewed").innerHTML = await getAnswerChoicesHtmlReviewed("checkbox");
                document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
                document.getElementById("questionAnswersDivRevieved").removeAttribute("hidden");
                break;
            }
        case '2':
            {
                document.getElementById("questionAnswersDivReviewed").innerHTML = await getAnswerChoicesHtmlReviewed("radio");
                document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
                document.getElementById("questionAnswersDivRevieved").removeAttribute("hidden");
                break;

            }
        case '3':
            document.getElementById("questionAnswersDivRevieved").innerHTML =
                `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıtlar</label>
                <div class="form-check form-check-inline" id="choices">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice0"  onChange="updateCheckedAnswersReviewed()">
                        <label class="form-check-label" for="true">True</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice1"  onChange="updateCheckedAnswersReviewed()">
                        <label class="form-check-label" for="false">False</label>
                    </div>
                </div>`;
            document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
            document.getElementById("questionAnswersDivRevieved").removeAttribute("hidden");
            break;
        case '4':
            document.getElementById("questionAnswersDivRevieved").innerHTML = await getClassicQuestionAnswerReviewed();
            document.getElementById("questionAnswersDivRevieved").removeAttribute("hidden");
            break;
        default:
            document.getElementById("questionAnswersDivRevieved").setAttribute("hidden", true);
            break;
    }
}

async function getClassicQuestionAnswerReviewed() {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="QuestionAnswers" id="classicQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers" oninput="updateQuestionAnswerChoicesReviewed(this.value)"></textarea>`;
}
function getClassicUpdateQuestionAnswerReviewed(questionAnswerText) {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="QuestionAnswers" id="classicQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers"  oninput="updateQuestionAnswerChoicesReviewed(this.value)">${JSON.parse(questionAnswerText)[0].Answer}</textarea>`;
}
function updateQuestionAnswerChoicesReviewed(value) {
    questionAnswerChoicesReviewed = [];
    questionAnswerChoicesReviewed.push({ Answer: value, IsRightAnswer: true });
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
}

async function getAnswerChoicesHtmlReviewed(choiceType) {
    let answerChoicesHtmlReviewed = `<div class="row">
        <label class="col-sm-6 col-form-label col-form-label-lg" for="QuestionAnswers">${localizedStrings.answers}</label>
        <div class="col-sm-6 col-form-label col-form-label-lg" style="display:flex; justify-content:end;">
            <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerTypeReviewed" id="stringRadio" ${(answerTypeNameReviewed === "Metin" || answerTypeNameReviewed === "Text") ? 'checked' : ''} onchange="updateAnswerTypeReviewed()">
            <label class="form-check-label" style="margin-right:5px;" for="stringRadio">${localizedStrings.text}</label>
            <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerTypeReviewed" id="imageRadio" ${(answerTypeNameReviewed === "Resim" || answerTypeNameReviewed === "Image") ? 'checked' : ''} onchange="updateAnswerTypeReviewed()">
            <label class="form-check-label" style="margin-right:5px;" for="imageRadio">${localizedStrings.image}</label>
        </div>
    </div>`;

    if (answerTypeNameReviewed === "Metin" || answerTypeNameReviewed === localizedStrings.text) {
        answerChoicesHtmlReviewed += questionAnswerChoicesReviewed.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceType}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswersReviewed()">
                    </div>
                    <div class="col-sm-9 col-12">
                       <textarea type="text" class="form-control form-control-solid" id="answerText${index}" placeholder="${localizedStrings.newoption}" value="${item.Answer}" onChange="updateAnswerTextReviewed(${index})">${item.Answer}</textarea>
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoiceReviewed(${index},'${choiceType}')"> X </button>
                    </div>
                </div>
            </div>`;
        }).join("");
        answerChoicesHtmlReviewed += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoiceReviewed('${choiceType}','${localizedStrings.text}')">${localizedStrings.addnewoption}</button>`;
    } else if (answerTypeNameReviewed === "Resim" || answerTypeNameReviewed === localizedStrings.image) {
        answerChoicesHtmlReviewed += questionAnswerChoicesReviewed.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceType}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswersReviewed()">
                    </div>
                    <div class="col-sm-9 col-12">
                        <div class="row">
                            <div class="${item.Answer ? `col-sm-9` : `col-sm-12`}" id="answerImageDivReviewed${index}">
                                 <input  type="file" class="form-control form-control-lg form-control-solid" id="file-input-reviewed-${index}" accept=".png,.jpeg" onClick="setupFileInputReviewed(${index})" />
                            </div>                 
                            <div class="col-sm-3">
                                <div id="image-preview-reviewed-${index}">
                                     ${item.Answer ? ` <img class="img-fluid img-thumbnail" style="max-height:100px;" src="${item.Answer}" alt="Uploaded Image">` : ``}
                                </div>              
                            </div>  
                        </div>                              
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoiceReviewed(${index},'${choiceType}')"> X </button>
                    </div>
                </div>
            </div>`
        }).join("");
        answerChoicesHtmlReviewed += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoiceReviewed('${choiceType}','${localizedStrings.image}')">${localizedStrings.addnewoption}</button>`;
    }

    return answerChoicesHtmlReviewed;
}

// Add this line at the beginning of your file
function getAnswerChoicesHtmlQuestionReviewed(choiceTypex, getquestionAnswerChoicesReviewed) {
    var choiceTypex = choiceTypex == 1 ? "checkbox" : "radio";

    questionAnswerChoicesReviewed = JSON.parse(getquestionAnswerChoicesReviewed);

    let answerChoicesHtmlReviewed = `<div class="row">
        <label class="col-sm-6 col-form-label col-form-label-lg" for="QuestionAnswers">${localizedStrings.answers}</label>
        <div class="col-sm-6 col-form-label col-form-label-lg" style="display:flex; justify-content:end;">
            <input class="form-check-input" style="margin-right:5px;" type="radio" name="answerTypeReviewed" id="stringRadio" ${questionAnswerChoicesReviewed[0].IsAnswerImage === false ? 'checked' : ''} onchange="updateAnswerTypeReviewed()">
            <label class="form-check-label" style="margin-right:5px;" for="stringRadio">${localizedStrings.text}</label>
            <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerTypeReviewed" id="imageRadio" ${questionAnswerChoicesReviewed[0].IsAnswerImage === true ? 'checked' : ''} onchange="updateAnswerTypeReviewed()">
            <label class="form-check-label" style="margin-right:5px;" for="imageRadio">${localizedStrings.image}</label>
        </div>
    </div>`;
    if (questionAnswerChoicesReviewed[0].IsAnswerImage === false) {
        answerChoicesHtmlReviewed += questionAnswerChoicesReviewed.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceTypex}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswersReviewed()">
                    </div>
                    <div class="col-sm-9 col-12">
                       <textarea type="text" class="form-control form-control-solid" id="answerText${index}" placeholder="${localizedStrings.newoption}" value="${item.Answer}" onChange="updateAnswerTextReviewed(${index})">${item.Answer}</textarea>
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoiceReviewed(${index},'${choiceTypex}')"> X </button>
                    </div>
                </div>
            </div>`;
        }).join("");
        answerChoicesHtmlReviewed += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoiceReviewed('${choiceTypex}','${localizedStrings.text}')">${localizedStrings.addnewoption}</button>`;
    } else if (questionAnswerChoicesReviewed[0].IsAnswerImage === true) {
        answerChoicesHtmlReviewed += questionAnswerChoicesReviewed.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceTypex}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswersReviewed()">
                    </div>
                    <div class="col-sm-9 col-12">
                        <div class="row">
                            <div class="col-sm-9 d-flex align-items-center" id="answerImageDivReviewed${index}">
                                 <input  type="file" class="form-control form-control-lg form-control-solid" id="file-input-reviewed-${index}" accept=".png,.jpeg" onClick="setupFileInputReviewed(${index})" />
                            </div>                 
                            <div class="col-sm-3">
                                <div id="image-preview-reviewed-${index}">
                                    <img class="img-fluid img-thumbnail" style="max-height:100px;" src="${item.Answer}" alt="Uploaded Image">
                                </div>              
                            </div>  
                        </div>                              
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoiceReviewed(${index},'${choiceTypex}')"> X </button>
                    </div>
                </div>
            </div>`
        }).join("");
        answerChoicesHtmlReviewed += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoiceReviewed('${choiceTypex}','${localizedStrings.image}')">${localizedStrings.addnewoption}</button>`;
    }

    return answerChoicesHtmlReviewed;

}
function getCheckboxReviewed(getquestionAnswerChoicesReviewed) {
    var questionAnswerChoicesReviewed = JSON.parse(questionAnswerChoicesReviewed);
    // Şimdi questionAnswerChoices bir JavaScript nesnesi ve üzerinde işlem yapabilirsiniz

    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıtlar</label>
                <div class="form-check form-check-inline" id="choices">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" ${questionAnswerChoicesReviewed[0].IsRightAnswer ? 'checked' : ''}  name="answerOptions" id="choice0"  onChange="updateCheckedAnswersReviewed()">
                        <label class="form-check-label" for="true">True</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice1" ${questionAnswerChoicesReviewed[1].IsRightAnswer ? 'checked' : ''}  onChange="updateCheckedAnswersReviewed()">
                        <label class="form-check-label" for="false">False</label>
                    </div>
                </div>`
}
function updateAnswerTextReviewed(index) {
    questionAnswerChoicesReviewed[index].Answer = document.getElementById(`answerText${index}`).value;
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
}
function setupFileInputReviewed(index) {
    const fileInput = document.getElementById(`file-input-reviewed-${index}`);
    const imagePreview = document.getElementById(`image-preview-reviewed-${index}`);
    const imageDiv = document.getElementById(`answerImageDivReviewed${index}`);
    fileInput.addEventListener('change', function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageUrl = e.target.result;
                imageDiv.className = "col-sm-9 d-flex align-items-center";
                imagePreview.innerHTML = `<img class="img-fluid img-thumbnail" style="max-height:100px;" src="${imageUrl}" alt="Uploaded Image">`;
                questionAnswerChoicesReviewed[index].Answer = imageUrl;
                questionAnswerChoicesReviewed[index].IsAnswerImage = true;
                document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
            };
            reader.readAsDataURL(file);
        } else {
            imagePreview.innerHTML = ""
        }
    });
}
async function updateAnswerTypeReviewed() {
    const radioButtons = document.getElementsByName("answerTypeReviewed");
    for (const button of radioButtons) {
        if (button.checked) {
            answerTypeNameReviewed = button.nextElementSibling.textContent.trim();
            questionAnswerChoicesReviewed.forEach((questionAnswerChoice, index) => {
                questionAnswerChoice.Answer = "";
                questionAnswerChoice.IsAnswerImage = false;
            })
        }
    }
    document.getElementById("questionAnswersDivReviewed").innerHTML = await getAnswerChoicesHtmlReviewed("radio");
}
function updateCheckedAnswersReviewed() {
    questionAnswerChoicesReviewed.forEach((item, index) => {
        item.IsRightAnswer = document.getElementById(`choice${index}`).checked;
    });
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
}
async function addNewChoiceReviewed(choiceType, answerTypeNameReviewed2) {
    answerTypeNameReviewed = answerTypeNameReviewed2;
    questionAnswerChoicesReviewed.push({ Answer: "", IsRightAnswer: false });
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
    document.getElementById("questionAnswersDivReviewed").innerHTML = await getAnswerChoicesHtmlReviewed(choiceType);

    questionAnswerChoicesReviewed.forEach((item, index) => {
        document.getElementById(`answerText${index}`).placeholder = "Yeni Seçenek";
    });
}
async function addChoiceReviewed(choiceType, getquestionAnswerChoicesReviewed) {
    getquestionAnswerChoicesReviewed.push({ Answer: "", IsRightAnswer: false, Id: "", QuestionId: "" });
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(getquestionAnswerChoicesReviewed);
    var buttonElement = document.getElementById("submitButton");
    document.getElementById("questionAnswersDivReviewed").removeChild(buttonElement);
    document.getElementById("questionAnswersDivReviewed").innerHTML += await getAnswerChoicesHtmlReviewed(choiceType);

    getquestionAnswerChoicesReviewed.forEach((item, index) => {
        document.getElementById(`answerText${index}`).placeholder = "Yeni Seçenek";
    });
}

async function removeChoiceReviewed(index, choiceType) {
    questionAnswerChoicesReviewed.splice(index, 1);
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
    document.getElementById("questionAnswersDivReviewed").innerHTML = await getAnswerChoicesHtmlReviewed(choiceType);
}

//Function for populating select lists
async function populateSelectListReviewed(selectListId, data) {
    let selectListOptions = data.map((item, index) => `<option value="${item.value}">${item.text}</option>`);
    let selectList = `<option value="" disabled="" selected="">--- Seçiniz ---</option>`.concat(selectListOptions);
    document.getElementById(selectListId).innerHTML = selectList;
}


$(document).ready(function () {
});

$("#productıd").change(function () {
});

$("button[type='submit']").click(function (event) {
    document.getElementById("questionAnswerChoicesReviewed").value = JSON.stringify(questionAnswerChoicesReviewed);
    event.preventDefault();

});

$(document).ready(function () {
    validateCheckBoxesReviewed();
});

//$("#ProductId").change(function () {
//    validateCheckBoxes();
//});

src = "~/lib/limonte-sweetalert2/sweetalert2.all.js";

function validateCheckBoxesReviewed() {
    let questionType = $("#questionTypeReviewed").val();
    if (questionType === "1" || questionType === "2" || questionType === "3" || questionType === "4") {
        if (questionType !== "4" && $("input[name='answerOptions']:checked").length === 0 || questionType === "4" && $("#classicQuestionAnswer").val().length === 0) {
            Swal.fire({
                title: 'Eksik Veri Girildi',
                text: 'Sorunun Doğru Cevabını veya Soru Bilgilerinizin Tamamını Girmediniz!',
                icon: 'error'
            });
            return false;
        }
        else {
            return true;
        }
    }
}

function removeChooseOptionReviewed(elementId) {
    var selectElement = document.getElementById(elementId);
    var chooseOption = selectElement.querySelector('option[value=""]');

    if (chooseOption) {
        selectElement.removeChild(chooseOption);
    }
}