import { useAppSelector } from '../../hooks';
import { MessageData } from '../../models/form.model';
import { convertDate } from '../../utils/convertDateToStr';

const ChatHistory = () => {
  const { messagesArr } = useAppSelector((state) => state.message);

  return (
    <div className="chat-bot__message">
      <div>
        {messagesArr.map((message: MessageData) => {
          const dateObj: Date = new Date(message.date);
          const dateStr = convertDate(dateObj);

          return (
            <div
              key={message.date}
              className={`msg-item ${message.isUser ? 'msg-user' : 'msg-bot'}`}
            >
              {!message.isUser && <div>{message.author}</div>}
              <div className="msg-date">{dateStr}</div>
              <div>{message.text}</div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default ChatHistory;
