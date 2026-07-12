const form = document.querySelector(".auth-form");

form.addEventListener("submit", async function(e){

    e.preventDefault();

    const inputs = form.querySelectorAll("input");

    const data = {

        firstName: inputs[0].value,

        lastName: inputs[1].value,

        email: inputs[2].value,

        password: inputs[3].value

    };

    const response = await fetch("http://localhost:5251/api/users/register",{

        method:"POST",

        headers:{
            "Content-Type":"application/json"
        },

        body: JSON.stringify(data)

    });

    const result = await response.json();

    if(response.ok){

        alert("Регистрацията е успешна!");

        window.location.href = "login.html";

    }
    else{

        alert(result.message);

    }

});