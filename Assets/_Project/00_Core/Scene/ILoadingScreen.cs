using Cysharp.Threading.Tasks;

namespace TSI.Core.Scene
{
	public interface ILoadingScreen
    {
		UniTask Show();
		UniTask Hide();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="progress">정규화된 로드 진행도 (0.0~1.0)</param>
		void SetProgress(float progress); 
    }
}