export const convertDate = (dateObj: Date): string => {
  const dateStr = `${dateObj.getFullYear()}-${convertDateStr(
    dateObj.getMonth() + 1
  )}-${convertDateStr(dateObj.getDate())} ${convertDateStr(dateObj.getHours())}:${convertDateStr(
    dateObj.getMinutes()
  )}`;

  return dateStr;
};

const convertDateStr = (num: number): string => {
  let result = '';
  if (num < 10) {
    result += '0';
  }

  result += num;
  return result;
};
