(
   start "task1" cmd /C "cd frontend & npm start"
   start "task2" cmd /C "cd backend & npm run start"
) | pause
