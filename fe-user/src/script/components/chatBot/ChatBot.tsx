import { SubmitHandler, useForm } from 'react-hook-form';
import { FormValues, MessageData } from '../../models/form.model';
import ChatHistory from './ChatHistory';
import { useAppDispatch } from '../../hooks';
import { addMessage } from '../../store/messageSlice';

const ChatBot = ({ closeBot }: { closeBot: () => void }) => {
  const dispatch = useAppDispatch();
  const { register, handleSubmit, reset } = useForm<FormValues>({ mode: 'onSubmit' });

  const closeChatBot = () => {
    closeBot();
  };

  const handleSubmitForm: SubmitHandler<FormValues> = (data) => {
    const { message } = data;
    if (!message) return;

    console.log('send request', message);
    const newMessage: MessageData = {
      date: Date.now(),
      author: '',
      isUser: true,
      text: message,
    };

    dispatch(addMessage(newMessage));
    reset();

    setTimeout(() => {
      console.log('get answer');
      const newBotMessage: MessageData = {
        date: Date.now(),
        author: 'Bot',
        isUser: false,
        text: 'Ми спробуємо Вам допомогти',
      };

      dispatch(addMessage(newBotMessage));
    }, 500);
  };

  return (
    <div className="chat-bot">
      <div className="chat-bot__close">
        <button className="chat-bot__close-btn" onClick={closeChatBot}>
          ✖
        </button>
      </div>
      <h4 className="chat-bot__title">Чат-бот</h4>
      <ChatHistory />
      <form className="chat-bot__form" onSubmit={handleSubmit(handleSubmitForm)}>
        <input className="chat-bot__input" type="text" {...register('message')} />
        <button type="submit" className="chat-bot__submit">
          {'➤'}
        </button>
      </form>
    </div>
  );
};

export default ChatBot;
