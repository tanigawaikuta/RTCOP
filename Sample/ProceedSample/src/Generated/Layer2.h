#ifndef __RTCOP_GENERATED_LAYER2__
#define __RTCOP_GENERATED_LAYER2__

#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"


#include "./Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

class Layer2 : public RTCOP::Core::Layer
{
public:
	static Layer2* GetInstance();
	explicit Layer2(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~Layer2() override;
protected:
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
	virtual void _RTCOP_OnActivating() override;
	virtual void _RTCOP_OnActivated() override;
	virtual void _RTCOP_OnDeactivating() override;
	virtual void _RTCOP_OnDeactivated() override;
};

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer2
{
class A : public RTCOP::Core::LayerdObject<baselayer::A>
{
public:
	friend RTCOP::Generated::Layer2;
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
	};
	A ();
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
	public:
	virtual void m1 ();

};
class B : public RTCOP::Core::LayerdObject<baselayer::B>
{
public:
	friend RTCOP::Generated::Layer2;
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
	};
	B ();
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
	public:
	virtual void m2 ();

};

}


#endif
