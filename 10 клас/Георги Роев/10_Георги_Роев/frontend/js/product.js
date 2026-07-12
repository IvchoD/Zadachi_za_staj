const params = new URLSearchParams(window.location.search);

const productId = params.get("id");

const container = document.getElementById("productDetails");

loadProduct();

async function loadProduct(){

    const response = await fetch(

        `http://localhost:5251/api/products/${productId}`

    );

    const product = await response.json();

    renderProduct(product);

}

function renderProduct(product){

    let stock = "";

    if(product.quantity > 10){

        stock = "<p class='stock in-stock'>🟢 В наличност</p>";

    }
    else if(product.quantity > 1){

        stock = `<p class='stock low-stock'>🟡 Остават само ${product.quantity} броя!</p>`;

    }
    else{

        stock = "<p class='stock last-item'>🔴 Последен буркан!</p>";

    }

    const badges = product.badges
        .map(b => `<span class="product-badge">${b.name}</span>`)
        .join("");

    container.innerHTML = `

    <div class="product-page">

        <div class="product-image">

            <img
                src="images/products/${product.imagePath}"
                alt="${product.name}"
            >

        </div>

        <div class="product-info">

            <h1>${product.name}</h1>

            <div class="product-badges">

                ${badges}

            </div>

            <h2>${product.price.toFixed(2)} €</h2>

            <p>

                ${product.description}

            </p>

            <h3>Съставки</h3>

            <p>

                ${product.ingredients}

            </p>

            ${stock}

            <button onclick="addToCart(${product.id})">

                Добави в количката

            </button>

            <br><br>

            <a class="back-button" href="index.html">

                ← Назад към продуктите

            </a>

        </div>

    </div>

    `;

}