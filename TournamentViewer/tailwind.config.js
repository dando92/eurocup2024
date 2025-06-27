/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        blu: '#0c4969',
        bianco: '#F3F9FD',
        giallo: '#FFCC07',
        nero: '#142739',
        grigio: '#E9E9E9',
        lower: '#2E9BD5',
        middle: '#0c4969',
        upper: '#0E1928',
        grigioQualifica: '#444443',
        gialloQualifica: '#FFC505',
      }
    },
  },
  plugins: [],
}

