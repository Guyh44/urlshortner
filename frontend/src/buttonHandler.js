//async for the await (waits for response) command 
export default async function handleSubmit() {

    const urlInput = document.querySelector(".input");
    const codeInput = document.querySelector(".input-short-code");

    const url = urlInput ? urlInput.value.trim() : "";
    const customCode = codeInput ? codeInput.value.trim() : "";
    //check for urls
    if (!url) {
        alert("Please enter a URL");
        return;
    }

    try {
        if (customCode === "") // no custom url was inputed
            await handleRandomShotCode(url)
        else
            await handleCustomShortCode(url, customCode)

    } catch (error) {
        console.error("Error sending request:", error);
        alert("error");
    }
    
}

async function handleRandomShotCode(url)
{
    const response = await fetch("http://localhost:5235/api/shorten", {
        method: "POST",
        headers: {
            "Content-Type": "application/json" //tels im sending json
        },
        body: JSON.stringify({ url })
    });

    if (!response.ok) {
        throw new Error(`Server error: ${response.status}`);
    }

    const result = await response.json(); // get the response
    console.log("Full response JSON:", result);
    console.log("Shortened URL:", result.shortUrl);
    alert(result.shortUrl)
}


async function handleCustomShortCode(url, customCode)
{
    const response = await fetch("http://localhost:5235/api/custom/shorten", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ url, customCode }) 
    });

    if (!response.ok) {
        throw new Error(`Server error: ${response.status}`);
    }

    const result = await response.json(); // get the response
    console.log("Full response JSON:", result);
    console.log("Shortened URL:", result.shortUrl);
    alert(result.shortUrl)
}