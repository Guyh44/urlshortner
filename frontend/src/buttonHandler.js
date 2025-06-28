import {
    shortenWithRandomCode,
    shortenWithCustomCode,
} from "./urlShortnerApi";

/**
 * Decides which API call to use based on customCode.
 * Parses response or throws custom error messages.
 */
export default async function handleSubmit(url, customCode, ttl) {
    if (!url.trim()) {
        throw new Error("Please enter a URL");
    }

    let response;
    try {
        if (customCode.trim() === "") {
            response = await shortenWithRandomCode(url.trim(), ttl);
        } else {
            response = await shortenWithCustomCode(url.trim(), customCode.trim(), ttl);
        }

        if (!response.ok) {
            if (response.status === 409) {
                throw new Error("Custom code already taken");
            }
            if (response.status === 400) {
                throw new Error("Invalid URL format");
            }
            throw new Error(`Server error: ${response.status}`);
        }

        const result = await response.json();
        return result;
    } catch (error) {
        console.error("Request failed:", error);
        throw error;
    }
}
