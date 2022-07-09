const url = window.location.origin
const txtMessage = document.getElementById('txtMessage')
const progressStatus = document.getElementById('progressStatus')
const progress_wrapper = document.getElementById('progressWrapper')
const progress = document.getElementById('progress')
const input = document.getElementById('fileSelector')
const fileInputLabel = document.getElementById('fileInputLabel')

const showAlert = (msg, bootstrapType) => {
    txtMessage.style.display = "block"
    txtMessage.innerHTML = `
    <div class="alert alert-${bootstrapType} alert-dismissible fade show">
    <button type="button" class="close" data-bs-dismiss="alert">&times;</button>
    ${msg}
  </div>
    `
}

const upload = () => {
    console.log('Here')
    txtMessage.style.display = "none"

    if (input.value == undefined || input.value == "") {
        showAlert("No File Selected", "danger")
        return;
    }

    let name = input.value.split('.')
    if (name[name.length - 1] != "xlsx") {
        input.value = ""
        showAlert("Plase Select xlsx File", "danger")
        return;
    }

    const file = input.files[0];
    let formData = new FormData()
    formData.append("file", file);

    input.disabled = true
    let xhr = new XMLHttpRequest();
    xhr.open("POST", url);
    xhr.responseType = 'json';
    xhr.setRequestHeader("RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val());

    xhr.onload = () => {
        reset()
        if (xhr.status >= 400) {
            showAlert("Failed", "danger");
            return;
        }

        alert("File uploaded successfully");
        window.location.href = url;
    }

    xhr.onprogress = (e) => {
        let loaded = e.loaded
        let total = e.total

        let done = (loaded / total) * 100

        progress.style.width = Math.floor(done)
        progressStatus.innerText = `${Math.floor(done)}% uploaded`

    }

    xhr.send(formData);
    return;
};

const reset = () => {
    input.disabled = false
    input.value = null
    progress_wrapper.classList.add('d-none')
    progress.style.width = 0
    fileInputLabel.innerText = "Select file"
}

const get_filename = () => {
    fileInputLabel.innerText = input.files[0].name
}

window.addEventListener("load", () => {
    input.value = ""
})

$(function () {
    var filterBtn = document.getElementById('filterBtn')
    filterBtn.addEventListener('click', (e) => {
        e.preventDefault();

        var filterString = document.getElementById('filterString').value;
        window.location.href = window.location.origin + `?handler=filter&filterString=${filterString}`
    });
});

const toggle = (id) => {
    window.location.href = window.location.origin + `?handler=toggle&id=${id}`
}