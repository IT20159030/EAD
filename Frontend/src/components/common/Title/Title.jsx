import { Button } from "react-bootstrap";
import styles from "./Title.module.css"; // Import the CSS module

const Title = ({ title, buttonLabel, onButtonClick }) => {
  return (
    <div className={styles.commonTitle}>
      <div className={styles.titleContainer}>
        <h1 className={styles.title}>{title}</h1>
        <div className={styles.buttonContainer}>
          {buttonLabel && (
            <Button variant="primary" onClick={onButtonClick}>
              {buttonLabel}
            </Button>
          )}
        </div>
      </div>
      <hr className={styles.separator} />
    </div>
  );
};

export default Title;
