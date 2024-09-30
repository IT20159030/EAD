export const openCloudinaryWidget = (setUrlCallback) => {
  const myWidget = window.cloudinary.createUploadWidget(
    {
      cloudName: import.meta.env.VITE_CLOUDINARY_CLOUD_NAME,
      uploadPreset: import.meta.env.VITE_CLOUDINARY_UPLOAD_PRESET,
      upload_single: true,
      sources: ["local", "camera"],
      resourceType: "image",
      clientAllowedFormats: ["image"],
    },
    (error, result) => {
      if (!error && result && result.event === "success") {
        setUrlCallback(result.info.url);
      }
    }
  );
  myWidget.open();
};
