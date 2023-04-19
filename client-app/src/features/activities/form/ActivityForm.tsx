
import { ChangeEvent, useEffect, useState } from "react";
import { Button, FormField, Header, Label, Segment } from "semantic-ui-react";
import { useStore } from "../../../app/strores/store";
import { observer } from "mobx-react-lite";
import { Link, useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { v4 as uuid } from "uuid";
import { redirect } from "react-router-dom";
import { Formik, Form, Field, ErrorMessage } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { CategoryOptions } from "../../../app/common/options/categoryOptions";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { Activity } from "../../../app/models/activity";

export default observer(function ActivityForm() {

    const { activityStore } = useStore();
    const { createActivity, updateActivity, loading, loadActivity, loadingInitial } = activityStore;
    const { id } = useParams<{ id: string }>();
    const [activity, setActivity] = useState<Activity>({
        id: '',
        title: '',
        category: '',
        description: '',
        date: null,
        city: '',
        venue: ''
    });

    const validationSchema = Yup.object({
        title: Yup.string().required('The activity title is required!'),
        description: Yup.string().required('The activity description is required!'),
        category: Yup.string().required(),
        date: Yup.string().required("Date is required!"),
        venue: Yup.string().required(),
        city: Yup.string().required(),
    })

    useEffect(() => {
        if (id) {
            loadActivity(id).then(activity => setActivity(activity!));
        }
    }, [id, loadActivity]);
    

    function handleFormSubmit(activity: Activity) {
        console.log("submit");
        if (activity.id.length === 0) {
            let newActivity = {
                ...activity,
                id: uuid()
            };
            createActivity(newActivity).then(() => redirect(`/activities/${newActivity.id}`));
        } else {
            updateActivity(activity).then(() => redirect(`/activities/${activity.id}`))
        }
    }

    // function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    //     const { name, value } = event.target;
    //     setActivity({ ...activity, [name]: value })
    // }

    if (loadingInitial) {
        return <LoadingComponent content="Loading activity..."/>
    }

    return (
        <Segment clearing>
            <Header sub color="teal">Activity details</Header>
            <Formik
                validationSchema={validationSchema}
                enableReinitialize
                initialValues={activity}
                onSubmit={values => handleFormSubmit(values)}>
                {({ handleSubmit, isValid, isSubmitting, dirty }) => (
                    <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
                        {/* <FormField>
                            <Field placeholder="Title" name='title' />
                            <ErrorMessage name='title'
                                render={error => <Label basic color="red" content={error} />} />
                        </FormField> */}
                        <MyTextInput name="title" placeholder="Title" />
                        <MyTextArea rows={3} placeholder="Description" name='description' />
                        <MySelectInput options={CategoryOptions} placeholder="Category" name='category'/>
                        <MyDateInput
                            showTimeSelect
                            placeholderText="Date"
                            timeCaption="Time"
                            dateFormat={"MMMM d, yyyy h:mm aa"}
                            name='date' />
                        <Header sub color="teal">Location details</Header>
                        <MyTextInput placeholder="City"  name='city' />
                        <MyTextInput placeholder="Venue"  name='venue' />
                        <Button disabled={isSubmitting || !dirty || !isValid} loading={loading} floated="right" positive type="submit" content='Submit' />
                        <Button as={Link} to={`/activities`} floated="right" type="button" content='Cancel' />
                    </Form>
                )}
            </Formik>
            
        </Segment>
    )
});