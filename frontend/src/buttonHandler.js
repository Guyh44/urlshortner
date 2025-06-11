//async for the await (waits for response) command 
export default async function handleSubmit() {

    const url = document.querySelector(".url-input").value;
    console.log("URL entered:", url);

    //check for urls
    if (!url) {
        alert("Please enter a URL");
        return;
    }

    try {
        const response = await fetch("http://localhost:5235/api/shorten", {
            method: "POST",
            headers: {
                "Content-Type": "application/json" //tels im sending json
            },
            body: JSON.stringify({ url: url }) // the json itself will be "url": "https://in&outUrl.com"
        });

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        const result = await response.json(); // get the response
        console.log("Full response JSON:", result);
        console.log("Shortened URL:", result.shortUrl);

    } catch (error) {
        console.error("Error sending request:", error);
        alert("error");
    }
    
}