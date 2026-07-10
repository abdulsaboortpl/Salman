/**
 * Reusable loading spinner for async operations.
 */
export default function LoadingSpinner({ message = 'Loading...', fullPage = false }) {
  const content = (
    <div className="text-center">
      <div className="spinner-border text-primary mb-3" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      <p className="text-muted mb-0">{message}</p>
    </div>
  );

  if (fullPage) {
    return (
      <div className="page-container justify-content-center">
        <div>{content}</div>
      </div>
    );
  }

  return content;
}
