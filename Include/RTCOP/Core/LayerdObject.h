//================================================================================
// LayerdObject.h
//
// 役割: レイヤードなオブジェクトを表すテンプレートクラス。
//       ユーザ定義のベースクラスを継承し、さらにレイヤードなオブジェクトに必要な要素が追加される。
//       RTCOPのインスタンス化では、ベースクラスを直接用いずに、必ずこのテンプレートクラスを用いること。
//================================================================================

#ifndef __RTCOP_CORE_LAYERDOBJECT__
#define __RTCOP_CORE_LAYERDOBJECT__

namespace RTCOP {
namespace Core {

class LayerdObject_Private;

// プライベートクラスの生成
LayerdObject_Private* LayerdObject_CreatePrivate();
// プライベートクラスの破棄
void LayerdObject_DestroyPrivate(LayerdObject_Private* privateClass);

// レイヤードなオブジェクトを表すテンプレートクラス
template<class Base>
class LayerdObject : public Base
{
public:
	// コンストラクタ (引数のパターンに合わせて用意する)
	template<class... ArgTypes>
	LayerdObject(ArgTypes... args) : Base(args...)
	{
		_Private = LayerdObject_CreatePrivate();
	}
	// デストラクタ
	virtual ~LayerdObject()
	{
		LayerdObject_DestroyPrivate(_Private);
		_Private = 0;
	}
	// ベースクラスのサイズの取得
	int GetBaseClassSize()
	{
		return sizeof(Base);
	}
protected:
	// プライベートクラス
	LayerdObject_Private* _Private;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERDOBJECT__
