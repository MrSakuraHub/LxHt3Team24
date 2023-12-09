import { MouseEventHandler, useState } from 'react';
import ChatBot from './ChatBot';

const Chat = () => {
  const [isShowButtons, setIsShowButtons] = useState(false);
  const [isShowBot, setIsShowBot] = useState(false);

  const handleMouseOver: MouseEventHandler = () => {
    setIsShowButtons(true);
  };

  const handleMouseOut: MouseEventHandler = () => {
    setIsShowButtons(false);
  };

  const openBot: MouseEventHandler<HTMLButtonElement> = () => {
    setIsShowBot(true);
  };

  const closeBot = (): void => {
    setIsShowBot(false);
    setIsShowButtons(false);
  };

  return (
    <div className="chat" onMouseOver={handleMouseOver} onMouseOut={handleMouseOut}>
      {isShowBot && <ChatBot closeBot={closeBot} />}
      {isShowButtons && !isShowBot && (
        <div className="chat__btn-container">
          <button className="chat__button btn-telegram">
            <a className="chat__link" target="_blank" href="https://t.me/csharp_e_Veteran_bot" />
          </button>
        </div>
      )}
      {!isShowBot && (
        <div className="chat__btn-container">
          <button className="chat__button btn-webchat" onClick={openBot} />
        </div>
      )}
    </div>
  );
};

export default Chat;
