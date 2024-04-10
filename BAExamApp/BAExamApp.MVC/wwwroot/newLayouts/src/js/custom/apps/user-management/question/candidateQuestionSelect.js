let subjects = [];
let subtopics = [];
let questionAnswerChoices = [];
let questionDifficulties = [];
let answerTypeName = "Metin";
document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);

function getAlgorithmUpdateQuestionAnswer(questionAnswerText) {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="QuestionAnswers" id="classicQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers"  oninput="updateAlgorithmQuestionAnswerChoices(this.value)"> ${questionAnswerText ? JSON.parse(questionAnswerText)[0].Answer : ""}</textarea>`;
}
function updateAlgorithmQuestionAnswerChoices(value) {
    questionAnswerChoices = [];
    questionAnswerChoices.push({ Answer: value, IsRightAnswer: true });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}



async function onQuestionTypeChange() {
    debugger
    var questionTypeSelect = document.getElementById('CandidateQuestionType').value;

    if (questionTypeSelect == 1) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }]
    }
    else if (questionTypeSelect == 2) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }];
    }
    else if (questionTypeSelect == 3) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }];

    }
    createQuestionAnswersHtml(questionTypeSelect);
};


async function createQuestionAnswersHtml(questionType) {
    debugger
    switch (questionType) {
        case '1':
            {
                document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml("radio");
                document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
                document.getElementById("questionAnswersDiv").removeAttribute("hidden");
                break;
            }
        case '2':
            {
                document.getElementById("questionAnswersDiv").innerHTML = await getAlgorithmUpdateQuestionAnswer();
                document.getElementById("questionAnswersDiv").removeAttribute("hidden");
                break;

            }
        case '3':
            document.getElementById("questionAnswersDiv").innerHTML = await getClassicUpdateQuestionAnswer();
            document.getElementById("questionAnswersDiv").removeAttribute("hidden");
            break;
        default:
            document.getElementById("questionAnswersDiv").setAttribute("hidden", true);
            break;
    }
}


function getClassicUpdateQuestionAnswer(getCandidateQuestionAnswersList) {

    if (getCandidateQuestionAnswersList) {
        questionAnswerChoices = JSON.parse(getCandidateQuestionAnswersList);
    }
    let classicAnswerHtml = `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt </label>`
    classicAnswerHtml += questionAnswerChoices.map((item, index) => {
        return `<div class="row g-3 align-items-center flex-start justify-content-center">
<div class="col-sm-11 col-12">
 <textarea name="questionAnswers" id="classicQuestionAnswer${index}" rows="2" class="form-control form-control-lg form-control-solid for="QuestionAnswers" oninput="updateQuestionAnswerChoices(this.value, ${index})" placeholder="Yanıt ${index + 1}" style="margin-bottom: 5px;">${item.Answer}</textarea></div>
  <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'text')"> X </button>
                    </div></div>`
            ;
    }).join("");
    classicAnswerHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('text')">Metin Cevap Ekle</button>`;
    return classicAnswerHtml;




}


async function getAnswerChoicesHtml(choiceType) {


    let answerChoicesHtml = `<label for="QuestionAnswers" class="col-sm-6 col-form-label col-form-label-lg">${localizedStrings.options}</label>`;

    answerChoicesHtml += questionAnswerChoices.map((item, index) => {
        return `<div class="form-check mb-3" id="choices"><div class="row g-3 flex-center justify-content-center">
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
    });
    answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceType}${localizedStrings.text}')">${localizedStrings.addnewoption}</button>`;

    return answerChoicesHtml;
}





function updateQuestionAnswerChoices(value, index) {
    questionAnswerChoices[index].Answer = value;
    questionAnswerChoices[index].IsRightAnswer = true;
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}

// Add this line at the beginning of your file
function getAnswerChoicesHtmlQuestion(choiceTypex, getquestionAnswerChoices) {
    var choiceTypex = "radio";
    questionAnswerChoices = JSON.parse(getquestionAnswerChoices);


    let answerChoicesHtml = `<label for="QuestionAnswers" class="col-sm-6 col-form-label col-form-label-lg">${localizedStrings.options}</label>`;


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
    answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceTypex}')">${localizedStrings.addnewoption}</button>`;

    return answerChoicesHtml;

}

function updateAnswerText(index) {
    questionAnswerChoices[index].Answer = document.getElementById(`answerText${index}`).value;
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}


function updateCheckedAnswers() {
    questionAnswerChoices.forEach((item, index) => {
        item.IsRightAnswer = document.getElementById(`choice${index}`).checked;
    });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}
async function addNewChoice(choiceType) {
    if (choiceType === "radio") {
        questionAnswerChoices.push({ Answer: "", IsRightAnswer: false });
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);

        //questionAnswerChoices.forEach((item, index) => {
        //    document.getElementById(`answerText${index}`).placeholder = localizer.newoption;
        //});
    } else if (choiceType === "classic" || choiceType === "text") {
        questionAnswerChoices.push({ Answer: "", IsRightAnswer: false });
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getClassicUpdateQuestionAnswer();

        //questionAnswerChoices.forEach((item, index) => {
        //    document.getElementById(`classicQuestionAnswer${index}`).placeholder = `Yanıt ${index + 1}`;
        //});
    }
    //questionAnswerChoices.forEach((item, index) => {
    //    document.getElementById(`answerText${index}`).placeholder = localizedStrings.newoption;
    //});
}
async function addChoice(choiceType, getquestionAnswerChoices) {
    getquestionAnswerChoices.push({ Answer: "", IsRightAnswer: false, Id: "", QuestionId: "" });
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

    if (choiceType === "radio") {
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);
    } else if (choiceType === "classic" || choiceType === "text") {
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getClassicUpdateQuestionAnswer();
    }
}

//Function for populating select lists
async function populateSelectList(selectListId, data) {
    let selectListOptions = data.map((item, index) => `<option value="${item.value}">${item.text}</option>`);
    let selectList = `<option value="" disabled="" selected="">${localizer.choose}</option>`.concat(selectListOptions);
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
    let questionType = $("#CandidateQuestionType").val();
    if (questionType === "1" || questionType === "2" || questionType === "3") {
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

function removeChooseOption(elementId) {
    var selectElement = document.getElementById(elementId);
    var chooseOption = selectElement.querySelector('option[value=""]');

    if (chooseOption) {
        selectElement.removeChild(chooseOption);
    }
}