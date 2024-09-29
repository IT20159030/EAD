import { Button } from "react-bootstrap";
import "./Title.css";

const Title = ({ title, buttonLabel, onButtonClick }) => {
  return (
    <div className="commonTitle">
      <div className="titleContainer">
        <h1 className="title">{title}</h1>
        <div className="buttonContainer">
          {buttonLabel && (
            <Button onClick={onButtonClick} className="button">
              {buttonLabel}
            </Button>
          )}
        </div>
      </div>
      <hr className="separator" />
    </div>
  );
};

export default Title;
