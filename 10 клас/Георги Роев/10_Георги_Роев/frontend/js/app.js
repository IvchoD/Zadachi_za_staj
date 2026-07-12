
// =================================
// Render Products
// =================================

const grid = document.getElementById("productGrid");


const searchInput = document.getElementById("searchInput");



function renderProducts(list){

    grid.innerHTML = "";

    list.forEach(product => {

        const shortDescription =
            product.description.length > 70
            ? product.description.substring(0,70) + "..."
            : product.description;

        grid.innerHTML += `

        <div class="product-card"
             onclick="openProduct(${product.id})">

            <img
                src="images/products/${product.imagePath}"
                alt="${product.name}"
            >

            <h3>${product.name}</h3>

            <p class="card-description">

                ${shortDescription}

            </p>

            <p class="price">

                ${product.price.toFixed(2)} €

            </p>

            <button
                onclick="event.stopPropagation(); return addToCart(${product.id})">

                Добави в количката

            </button>

        </div>

        `;

    });

}

async function loadProducts(){

    const response = await fetch("http://localhost:5251/api/products");

    products = await response.json();

    renderProducts(products);

}

loadProducts();

searchInput.addEventListener("input", () => {

    const value = searchInput.value.toLowerCase();

    const filtered = products.filter(product =>
        product.name.toLowerCase().includes(value)
    );

    renderProducts(filtered);

});


async function addToCart(productId) {

    console.log(JSON.stringify({

    userId:1,

    productId:productId,

    quantity:1

}));

const response = await fetch("http://localhost:5251/api/cart/add", {

        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({

            userId: 1,

            productId: productId,

            quantity: 1

        })

    });

    const result = await response.json();

console.log(result);

if(result.success){

    const toast = document.getElementById("toast");

    toast.classList.add("show");

    setTimeout(() => {

        toast.classList.remove("show");

    }, 2000);

}else{

    alert(result.message);

}

    return false;
}

function openProduct(productId){

    window.location.href = `product.html?id=${productId}`;

}