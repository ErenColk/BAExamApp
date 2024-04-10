let subjects = [];
let subtopics = [];
let questionAnswerChoices = [];
let questionDifficulties = [];
let answerTypeName = "Metin";
document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);



if ($("#QuestionType").val()) {
    questionAnswerChoices = JSON.parse(document.getElementById("questionAnswersListFromReload").value);
    createQuestionAnswersHtml($("#QuestionType").val());
}

//Question type change event.
async function onQuestionTypeChange() {
    let questionType = document.getElementById("QuestionType").value;

    if (questionType == 1) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }]
    }
    else if (questionType == 2) {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }, { Answer: "", IsRightAnswer: false }];
    }
    else {
        questionAnswerChoices = [{ Answer: "", IsRightAnswer: false }];
    }

    createQuestionAnswersHtml(questionType);
};

async function createQuestionAnswersHtml(questionType) {

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
                document.getElementById("questionAnswersDiv").innerHTML = await getAlgorithmQuestionAnswer(questionAnswerChoices[0]);
                document.getElementById("questionAnswersDiv").removeAttribute("hidden");
                break;

            }
        case '3':
            document.getElementById("questionAnswersDiv").innerHTML = await getClassicQuestionAnswer();


            document.getElementById("questionAnswersDiv").removeAttribute("hidden");
            break;
        default:
            document.getElementById("questionAnswersDiv").setAttribute("hidden", true);
            break;
    }
}
function getCheckbox(getquestionAnswerChoices) {

    // Şimdi questionAnswerChoices bir JavaScript nesnesi ve üzerinde işlem yapabilirsiniz

    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">${localizer.answers}</label>
                <div class="form-check form-check-inline" id="choices">
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" ${getquestionAnswerChoices[0].IsRightAnswer ? 'checked' : ''}  name="answerOptions" id="choice0"  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="true">True</label>
                    </div>
                    <div class="form-check form-check-inline">
                        <input class="form-check-input" type="radio" name="answerOptions" id="choice1" ${getquestionAnswerChoices[1].IsRightAnswer ? 'checked' : ''}  onChange="updateCheckedAnswers()">
                        <label class="form-check-label" for="false">False</label>
                    </div>
                </div>`
}

async function getClassicQuestionAnswer() {

    let classicAnswerHtml = `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt </label>`

    classicAnswerHtml += questionAnswerChoices.map((item, index) => {
        return `                <div class="row g-3 flex-center justify-content-center">
<div class="col-sm-9 col-12">
 <textarea name="questionAnswers" id="classicQuestionAnswer${index}" rows="2" class="form-control form-control-lg form-control-solid for="QuestionAnswers" oninput="updateQuestionAnswerChoices(this.value, ${index})" placeholder="Yanıt ${index + 1}" style="margin-bottom: 5px;">${item.Answer}</textarea></div>
  <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'text')"> X </button>
                    </div></div>`
            ;
    }).join("");
    classicAnswerHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('text')">Metin Cevap Ekle</button>`;

    return classicAnswerHtml;
}


function updateQuestionAnswerChoices(value, index) {
    questionAnswerChoices[index].Answer = value;
    questionAnswerChoices[index].IsRightAnswer = true;
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}



async function getAlgorithmQuestionAnswer(Answer) {
    return `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Yanıt</label>
                 <textarea name="questionAnswers" id="algorithmQuestionAnswer" rows="4" class="form-control form-control-lg form-control-solid for="QuestionAnswers" oninput="updateAlgorithmQuestionAnswerChoices(this.value)">${Answer.Answer}</textarea>`;
}

function updateAlgorithmQuestionAnswerChoices(value) {
    questionAnswerChoices = [];
    questionAnswerChoices.push({ Answer: value, IsRightAnswer: true });
    document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
}


async function getAnswerChoicesHtml(choiceType) {
    let answerChoicesHtml= `<label class="col-sm-12 col-form-label col-form-label-lg" for="QuestionAnswers">Seçenekler</label>`


     answerChoicesHtml += questionAnswerChoices.map((item, index) => {
        return `<div class="form-check mb-3" id="choices">
                <div class="row g-3 flex-center justify-content-center">
                    <div class="form-check col-sm-1 col-1 offset-xs-5 offset-sm-0">
                        <input class="form-check-input" type="${choiceType}" name="answerOptions" id="choice${index}" ${item.IsRightAnswer ? 'checked' : ''} onChange="updateCheckedAnswers()">
                    </div>
                    <div class="col-sm-9 col-12">
                       <textarea type="text" class="form-control form-control-solid" id="answerText${index}" placeholder="${localizer.newoption}" value="${item.Answer}" onChange="updateAnswerText(${index})">${item.Answer}</textarea>
                    </div>
                    <div class="col-sm-1 col-2 offset-4 offset-sm-0">
                        <button class="btn btn-danger btn-sm" type="button" onclick="removeChoice(${index},'${choiceType}')"> X </button>
                    </div>
                </div>
            </div>`;
    }).join("");
    answerChoicesHtml += `<button class="btn btn-primary btn-sm col-6 offset-3" type="button" onclick="addNewChoice('${choiceType}')">${localizer.addnewoption}</button>`;


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

async function addNewChoice(choiceType) {
    if (choiceType === "radio") {
        questionAnswerChoices.push({ Answer: "", IsRightAnswer: false });
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);
        questionAnswerChoices.forEach((item, index) => {
            document.getElementById(`answerText${index}`).placeholder = localizer.newoption;
        });
    } else if (choiceType === "classic" || choiceType === "text") {
        questionAnswerChoices.push({ Answer: "", IsRightAnswer: false });
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getClassicQuestionAnswer();
        questionAnswerChoices.forEach((item, index) => {
            document.getElementById(`classicQuestionAnswer${index}`).placeholder = `Yanıt ${index + 1}`;
        });
    }
}


async function removeChoice(index, choiceType) {
    questionAnswerChoices.splice(index, 1);
   

    if (choiceType === "radio") {
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getAnswerChoicesHtml(choiceType);
    } else if (choiceType === "classic" || choiceType === "text") {
        document.getElementById("questionAnswerChoices").value = JSON.stringify(questionAnswerChoices);
        document.getElementById("questionAnswersDiv").innerHTML = await getClassicQuestionAnswer();
    }

}

$(document).ready(function () {
});


$("button[type='submit']").click(function (event) {
    event.preventDefault();

    if (validateCheckBoxes()) {
        $("form").submit();
    }
});


src = "~/lib/limonte-sweetalert2/sweetalert2.all.js";

function validateCheckBoxes() {
    let questionType = $("#QuestionType").val();
    if (questionType === "1" || questionType === "2" || questionType === "3") {
        if ((questionType === "1" && $("input[name='answerOptions']:checked").length === 0) || (questionType === "2" && $("#algorithmQuestionAnswer").val().length === 0) || (questionType === "3" && $("#classicQuestionAnswer").val().length === 0)) {
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
