import jsPDF from "jspdf";
import "jspdf-autotable";

export const downloadPDF = (title, columns, data, fileName) => {
  const doc = new jsPDF();
  doc.text(title, 14, 16);
  doc.autoTable({
    startY: 20,
    head: [columns],
    body: data,
  });
  doc.save(fileName);
};
