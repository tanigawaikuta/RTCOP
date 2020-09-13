#ifndef __RTCOP_GENERATED_JAPANESE__
#define __RTCOP_GENERATED_JAPANESE__

#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"


#include "./src/Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

class Japanese : public RTCOP::Core::Layer
{
public:
	static Japanese* GetInstance();
	explicit Japanese(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~Japanese() override;
protected:
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
	virtual void _RTCOP_OnActivating() override;
	virtual void _RTCOP_OnActivated() override;
	virtual void _RTCOP_OnDeactivating() override;
	virtual void _RTCOP_OnDeactivated() override;
};

} // namespace Generated {}
} // namespace RTCOP {}

namespace Japanese
{
class Hello : public RTCOP::Core::LayerdObject<baselayer::Hello>
{
public:
	friend RTCOP::Generated::Japanese;
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
	};
	Hello ();
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
	public:
	virtual void Print ();

};

}


#endif
