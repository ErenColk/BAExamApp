﻿let subjects = [];
let subtopics = [];
let questionAnswerChoices = [];
let questionDifficulties = [];
let answerTypeName = "Metin";
document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);


async function createQuestionAnswersHtml(questionType) {
    switch (questionType) {
        case '1':
            {
                document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml("checkbox");
                document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
                document.getElementById("questionAnswersDiv").removeAttribute("hidden");
                break;
            }
        case '2':
            {
                document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml("radio");
                document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
                document.getElementById("questionAnswersDiv").removeAttribute("hidden");
                break;

            }
        case '3':
            document.getElementById("questionAnswersDiv").innerHTML =
                `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıtlar</label>
                <div class="form-check form-check-inline" id="choices">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice0"  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="true">${localizedStrings.true}</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice1"  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="false">${localizedStrings.false}</label>
                    </div>
                </div>`;
            document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
            document.getElementById("questionAnswersDiv").removeAttribute("hidden");
            break;
        case '4':
            document.getElementById("questionAnswersDiv").innerHTML = await getClassicQuestionAnswer();
            document.getElementById("questionAnswersDiv").removeAttribute("hidden");
            break;
        default:
            document.getElementById("questionAnswersDiv").setAttribute("hidden", true);
            break;
    }
}

async function getClassicQuestionAnswer() {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="QuestionAnswers" id="classicQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers" oninput="updateQuestionAnswerChoices(this.value)"></textarea>`;
}
 function getClassicUpdateQuestionAnswer(questionAnswerText) {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="QuestionAnswers" id="classicQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers"  oninput="updateQuestionAnswerChoices(this.value)">${JSON.parse(questionAnswerText)[0].Answer}</textarea>`;
}
function updateQuestionAnswerChoices(value) {
    questionAnswerChoices = [];
    questionAnswerChoices.push({ Answer: value, IsRightAnswer: true });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}

async function getAnswerChoicesHtml(choiceType) {
    let answerChoicesHtml = `<div class="row">
        <label class="col-sm-6 col-form-label col-form-label-lg" for="QuestionAnswers">${localizedStrings.answers}</label>
        <div class="col-sm-6 col-form-label col-form-label-lg" style="display:flex; justify-content:end;">
           <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerType" id="stringRadio" ${(answerTypeName === "Metin" || answerTypeName === localizedStrings.text) ? 'checked' : ''} onchange="updateAnswerType()">
            <label class="form-check-label" style="margin-right:5px;" for="stringRadio">${localizedStrings.text}</label>
            <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerType" id="imageRadio" ${(answerTypeName === "Resim" || answerTypeName === localizedStrings.image) ? 'checked' : ''} onchange="updateAnswerType()">
            <label class="form-check-label" style="margin-right:5px;" for="imageRadio">${localizedStrings.image}</label>
        </div>
    </div>`;

    if (answerTypeName === localizedStrings.text || answerTypeName === "Metin") {
        answerChoicesHtml += questionAnswerChoices.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceType}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswers()">
                    </div>
                    <div class="col-sm-9 col-12">
                       <textarea type="text" class="form-control form-control-solid" id="answerText${index}" placeholder="${localizedStrings.newoption}" value="${item.Answer}" onChange="updateAnswerText(${index})">${item.Answer}</textarea>
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'${choiceType}')"> X </button>
                    </div>
                </div>
            </div>`;
        }).join("");
        answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceType}','${localizedStrings.text}')">${localizedStrings.addnewoption}</button>`;
    } else if (answerTypeName === localizedStrings.image) {
        answerChoicesHtml += questionAnswerChoices.map((item, index) => {
            return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceType}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswers()">
                    </div>
                    <div class="col-sm-9 col-12">
                        <div class="row">
                            <div class="${item.Answer ? `col-sm-9` : `col-sm-12`}" id="answerImageDiv${index}">
                                 <input  type="file" class="form-control form-control-lg form-control-solid" id="file-input-${index}" accept=".png,.jpeg" onClick="setupFileInput(${index})" />
                            </div>                 
                            <div class="col-sm-3">
                                <div id="image-preview-${index}">
                                   ${item.Answer ? ` <img class="img-fluid img-thumbnail" style="max-height:100px;" src="${item.Answer}" alt="Uploaded Image">` : ``}
                                </div>              
                            </div>  
                        </div>                              
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'${choiceType}')"> X </button>
                    </div>
                </div>
            </div>`
        }).join("");
        answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceType}','${localizedStrings.image}')">${localizedStrings.addnewoption}</button>`;
    }

    return answerChoicesHtml;
}

// Add this line at the beginning of your file
  function getAnswerChoicesHtmlQuestion(choiceTypex, getquestionAnswerChoices) {
      var choiceTypex = choiceTypex == 1 ? "checkbox" : "radio";

      questionAnswerChoices = JSON.parse(getquestionAnswerChoices);
      let answerChoicesHtml = `<div class="row">
        <label class="col-sm-6 col-form-label col-form-label-lg" for="QuestionAnswers">${localizedStrings.answers}</label>
        <div class="col-sm-6 col-form-label col-form-label-lg" style="display:flex; justify-content:end;">
            <input class="form-check-input" style="margin-right:5px;" type="radio" name="answerType" id="stringRadio" ${ questionAnswerChoices[0].IsAnswerImage === false ? 'checked' : ''} onchange="updateAnswerType()">
            <label class="form-check-label" style="margin-right:5px;" for="stringRadio">${localizedStrings.text}</label>
            <input class="form-check-input" type="radio" style="margin-right:5px;" name="answerType" id="imageRadio" ${ questionAnswerChoices[0].IsAnswerImage === true ? 'checked' : ''} onchange="updateAnswerType()">
            <label class="form-check-label" style="margin-right:5px;" for="imageRadio">${localizedStrings.image}</label>
        </div>
    </div>`;
      if (questionAnswerChoices[0].IsAnswerImage === false) {
          answerChoicesHtml += questionAnswerChoices.map((item, index) => {
              return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceTypex}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswers()">
                    </div>
                    <div class="col-sm-9 col-12">
                       <textarea type="text" class="form-control form-control-solid" id="answerText${index}" placeholder="${localizedStrings.newoption}" value="${item.Answer}" onChange="updateAnswerText(${index})">${item.Answer}</textarea>
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'${choiceTypex}')"> X </button>
                    </div>
                </div>
            </div>`;
          }).join("");
          answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceTypex}','${localizedStrings.text}')">${localizedStrings.addnewoption}</button>`;
      } else if (questionAnswerChoices[0].IsAnswerImage === true) {
          answerChoicesHtml += questionAnswerChoices.map((item, index) => {
              return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceTypex}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswers()">
                    </div>
                    <div class="col-sm-9 col-12">
                        <div class="row">
                            <div class="col-sm-9 d-flex align-items-center" id="answerImageDiv${index}">
                                 <input  type="file" class="form-control form-control-lg form-control-solid" id="file-input-${index}" accept=".png,.jpeg" onClick="setupFileInput(${index})" />
                            </div>                 
                            <div class="col-sm-3">
                                <div id="image-preview-${index}">
                                    <img class="img-fluid img-thumbnail" style="max-height:100px;" src="${item.Answer}" alt="Uploaded Image">
                                </div>              
                            </div>  
                        </div>                              
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'${choiceTypex}')"> X </button>
                    </div>
                </div>
            </div>`
          }).join("");
          answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceTypex}','${localizedStrings.image}')">${localizedStrings.addnewoption}</button>`;
      }

      return answerChoicesHtml;
       
}
 function getCheckbox(getquestionAnswerChoices) {
     var questionAnswerChoices = JSON.parse(getquestionAnswerChoices);
// Şimdi questionAnswerChoices bir JavaScript nesnesi ve üzerinde işlem yapabilirsiniz

    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıtlar</label>
                <div class="form-check form-check-inline" id="choices">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" ${questionAnswerChoices[0].IsRightAnswer ? 'checked' : ''}  name="answerOptions" id="choice0"  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="true">${localizedStrings.true}</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice1" ${questionAnswerChoices[1].IsRightAnswer ? 'checked' : ''}  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="false">${localizedStrings.false}</label>
                    </div>
                </div>`
}
function updateAnswerText(index) {
    questionAnswerChoices[index].Answer = document.getElementById(`answerText${index}`).value;
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}
function setupFileInput(index) {
    const fileInput = document.getElementById(`file-input-${index}`);
    const imagePreview = document.getElementById(`image-preview-${index}`);
    const imageDiv = document.getElementById(`answerImageDiv${index}`);
    fileInput.addEventListener('change', function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageUrl = e.target.result;
                imageDiv.className = "col-sm-9 d-flex align-items-center";
                imagePreview.innerHTML = `<img class="img-fluid img-thumbnail" style="max-height:100px;" src="${imageUrl}" alt="Uploaded Image">`;
                questionAnswerChoices[index].Answer = imageUrl;
                questionAnswerChoices[index].IsAnswerImage = true;
                document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
            };
            reader.readAsDataURL(file);
        } else {
            imagePreview.innerHTML = ""
        }
    });
}
async function updateAnswerType() {
    const radioButtons = document.getElementsByName("answerType");
    for (const button of radioButtons) {
        if (button.checked) {
            answerTypeName = button.nextElementSibling.textContent.trim();
            questionAnswerChoices.forEach((questionAnswerChoice, index) => {
                questionAnswerChoice.Answer = "";
                questionAnswerChoice.IsAnswerImage = false;
            })
        }
    }
    document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml("radio");
}
function updateCheckedAnswers() {
    questionAnswerChoices.forEach((item, index) => {
        item.IsRightAnswer = document.getElementById(`choice${index}`).checked;
    });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}
async function addNewChoice(choiceType, answerTypeName2) {
    answerTypeName = answerTypeName2;
    questionAnswerChoices.push({ Answer: "", IsRightAnswer: false });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
    document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);

    //questionAnswerChoices.forEach((item, index) => {
    //    document.getElementById(`answerText${index}`).placeholder = localizedStrings.newoption;
    //});
}
async function addChoice(choiceType,getquestionAnswerChoices) {
    getquestionAnswerChoices.push({ Answer: "", IsRightAnswer: false,Id:"",QuestionId:"" });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(getquestionAnswerChoices);
    var buttonElement = document.getElementById("submitButton");
    document.getElementById("questionAnswersDiv").removeChild(buttonElement);
    document.getElementById("questionAnswersDiv").innerHTML += await getAnswerChoicesHtml(choiceType);

    getquestionAnswerChoices.forEach((item, index) => {
        document.getElementById(`answerText${index}`).placeholder = $localizedStrings.newoption;
    });
}

async function removeChoice(index, choiceType) {
    questionAnswerChoices.splice(index, 1);
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
    document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);
}

//Function for populating select lists
async function populateSelectList(selectListId, data) {
    let selectListOptions = data.map((item, index) => `<option value="${item.value}">${item.text}</option>`);
    let selectList = `<option value="" disabled="" selected="">${localizedStrings.choose}</option>`.concat(selectListOptions);
    document.getElementById(selectListId).innerHTML = selectList;
}


//$(document).ready(function () {
//});

//$("#productıd").change(function () {
//});

$("button[type='submit']").click(function (event) {
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
    event.preventDefault();

});

$(document).ready(function () {
    validateCheckBoxes();
});

//$("#ProductId").change(function () {
//    validateCheckBoxes();
//});

src = "~/lib/limonte-sweetalert2/sweetalert2.all.js";

function validateCheckBoxes() {
    let questionType = $("#QuestionType").val();
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

//Product type change event.
async function onProductChange() {
    subjects = subjects ? await getSubjects($("#ProductId").val()) : subjects;
    populateSelectList("SubjectId", subjects);
    subtopics = [];
    populateSelectList("SubtopicId", subtopics);
    removeChooseOption("SubjectId");
    removeChooseOption("SubtopicId");
};

//Product type change event.
async function onSubjectChange() {
    subtopics = subtopics ? await getSubtopics($("#SubjectId").val()) : subtopics;
    populateSelectList("SubtopicId", subtopics);
    removeChooseOption("SubtopicId");
};

async function getTime(selectedQuestionDifficultyId) {
    return $.ajax({
        url: '/Admin/Question/GetQuestionTimeByDifficultyId',
        data: { difficultyId: selectedQuestionDifficultyId },
    });
}

//Question type change event.
async function onQuestionTypeChange() {

    document.getElementById("TimeGiven").value = "";
    questionDifficulties = questionDifficulties ? await getQuestionDifficulties($("#QuestionType").val()) : questionDifficulties;
    
    populateSelectList("QuestionDifficultyId", questionDifficulties);
    questionDifficulties = [];

    let questionType = document.getElementById("QuestionType").value;
    
    if (questionType == 3) {
        questionAnswerChoices = [{ Answer: "True", IsRightAnswer: false }, { Answer: "False", IsRightAnswer: false }];
    } else if (questionType == 2) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }]
    }
    else {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }];
    }

    createQuestionAnswersHtml(questionType);
};


//Question difficulty change event.
async function onQuestionDifficultyChange() {
    let questionType = document.getElementById("QuestionType").value;

    if (questionType !== "4") {
        document.getElementById("TimeGiven").value = await getTime($("#QuestionDifficultyId").val());
    }
};

async function getQuestionDifficulties() {
    return $.ajax({
        url: '/Admin/Question/GetQuestionDifficulties'
    });
}

function removeChooseOption(elementId) {
    var selectElement = document.getElementById(elementId);
    var chooseOption = selectElement.querySelector('option[value=""]');

    if (chooseOption) {
        selectElement.removeChild(chooseOption);
    }
}